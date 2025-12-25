using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3.Board;
using Match3.Datas.Runtime;
using Match3.Factories;
using Match3.GameSystem;
using Match3.Models;
using Match3.Utils;
using Match3.Views;
using Match3.Views.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Match3.Factories.BlockFactory;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Controls
{
    public class BoardControl : Singleton<BoardControl>
    {
        [SerializeField] private SpriteRenderer _selectBorder;
        [SerializeField] private SpriteRenderer _hoverBorder;

        [Header("Audio")]
        [SerializeField] private AudioClip _swapClip;
        [SerializeField] private AudioClip _matchClip;

        public IngameBlockView SelectedBlock {get; private set;}
        private BoardView _boardView;
        private BoardModel _board;
        private bool _canInput = true;

        public UnityEvent OnMove {get; private set;} = new();
        public UnityEvent<List<Cell>> OnMatch {get; private set;} = new();
        public UnityEvent OnUpdateBoard {get; private set;} = new();
        
        #region Block Manage
        public IngameBlockView SpawnBlockView(BlockModel block, Cell pos)
        {
            var blockView = BlockFactory.Instance.GetIngameBlock(block, _boardView.Blocks);
            var worldPos = _boardView.CellToWorld(new(pos.Item1, pos.Item2));
            blockView.transform.position = worldPos;
            blockView.OnSelect.AddListener(() => SelectBlock(blockView));

            return blockView;
        }

        private BlockAbility GetAbility(BlockModel block)
        {
            return block.AbilityType switch
            { 
                EAbilityType.Vertical => new AxisSweep(block, false, true),
                EAbilityType.Horizontal => new AxisSweep(block, true, false),
                EAbilityType.Boom => new Boom(block),
                EAbilityType.ClearInstance => new ClearInstance(block),
                _ => null
            };
        }

        private void HandleAbilityMatch(BlockModel block1, BlockModel block2)
        {
            AudioControl.Instance.PlaySfx(_swapClip);

            var cell1 = _board.GetPosition(block1);
            var ability1 = GetAbility(block1);
            var ability2 = GetAbility(block2);

            block1.RemoveAbility();
            block2.RemoveAbility();

            BlockAbility combinedAbility;
            if (ability1 == null || ability2 == null)
            {
                combinedAbility = ability1 == null ? new ClearInstance(block2, block1) : new ClearInstance(block1, block2);
            }
            else
            { 
                combinedAbility = ability1.Combine(ability2);
            }
            
            combinedAbility.Do(_board, cell1, out var scoredPos);
            StartCoroutine(FXPlayer.Instance.PlayAbilityFX(combinedAbility, _boardView, cell1, scoredPos));

            OnMatch?.Invoke(scoredPos);
            foreach (var pos in scoredPos)
            {
                ScoreBlock(pos);
            }
            OnUpdateBoard?.Invoke();
        }

        private void HandleMatch(List<Cell> matchHor, List<Cell> matchVer)
        {
            AudioControl.Instance.PlaySfx(_swapClip);

            var intersectCell = matchHor.Intersect(matchVer).ToList()[0];
            // Debug.Log($"Match at {intersectCell}");
            if (_board.BlockDict[intersectCell] == null) 
            {
                Debug.Log($"Cant find block at {intersectCell}");
                return;
            }
            
            var intersectType = _board.BlockDict[intersectCell].Type;

            var matchPos = matchHor.Concat(matchVer).Distinct().ToList();
            
            OnMatch?.Invoke(matchPos);
            foreach(var pos in matchPos)
            {
                ScoreBlock(pos);
            }

            var special = SpecialAbilityMapping.GetAbilityType(matchHor.Count, matchVer.Count);
            if (SpecialAbilityMapping.GetAbilityType(matchHor.Count, matchVer.Count) != EAbilityType.None)
            {
                var specialBlockData = BlockFactory.Instance.GetBlockData(intersectType, special);
                BlockModel specialBlock = new(specialBlockData);
                _board.Set(intersectCell, specialBlock);

                SpawnBlockView(specialBlock, intersectCell);
            }

            OnUpdateBoard?.Invoke();
        }

        public IEnumerator ApplyGravity()
        {
            BoardGravity.ApplyGravity(_board, out var newBlocks);
            var offsetX = _board.Min.x;
            var offsetY = 0;
            for (int i = 0; i < newBlocks.Count; i++)
            {
                while (newBlocks[i].Count > 0)
                {
                    SpawnBlockView(newBlocks[i].Dequeue(), (i + offsetX, _board.Max.y + offsetY));
                    offsetY++;
                }

                offsetY = 0;
            }

            yield return StartCoroutine(SyncBoard(.2f));
            OnUpdateBoard?.Invoke();
        }

        public void SelectBlock(IngameBlockView block)
        {
            if (!_canInput) return;

            if (SelectedBlock == null) 
            {
                SelectedBlock = block;
                SelectedBlock.Select();
                _selectBorder.gameObject.SetActive(true);
                _selectBorder.transform.position = SelectedBlock.transform.position;
                return;
            }
            
            if (SelectedBlock == block)
            {
                // Debug.Log($"Deselect: {block.name}");
                SelectedBlock.Deselect();
                SelectedBlock = null;
                _selectBorder.gameObject.SetActive(false);
                return;
            }
            
            if (_boardView.IsAdjacent(SelectedBlock, block))
            {
                StartCoroutine(HandleSwapBlocks(block));
            }
            else
            {
                SelectedBlock.Deselect();
                SelectedBlock = block;
                SelectedBlock.Select();
                _selectBorder.gameObject.SetActive(true);
                _selectBorder.transform.position = SelectedBlock.transform.position;
            }
        }

        public void SetBlock(Cell pos, BlockModel toSet)
        {
            _board.Set(pos, toSet);
            _boardView.Set(new(pos.Item1, pos.Item2), toSet);
        }

        private void ScoreBlock(Cell pos)
        {
            var block = _board.BlockDict[pos];
            RemoveBlock(pos);

            if (block == null) return;
            
            var ability = GetAbility(block);
            if (ability != null)
            {
                ability.Do(_board, pos, out var scored);
                StartCoroutine(FXPlayer.Instance.PlayAbilityFX(ability, _boardView, pos, scored));
                scored.Remove(pos);

                foreach (var scoredPos in scored)
                {
                    ScoreBlock(scoredPos);
                }
            }
        }

        private void RemoveBlock(Cell pos)
        {
            _board.Remove(pos);
            // var blockView = _boardView.GetBlock(new(pos.Item1, pos.Item2));
            _boardView.Remove(new(pos.Item1, pos.Item2));
        }
        #endregion

        #region Board Manage
        public IEnumerator SyncBoard(float duration)
        // public async void SyncBoard(float duration)
        {
            yield return null;

            // List<Task> moveTask = new();
            var blockViews = _boardView.GetAllBlocks();
            foreach(var blockView in blockViews)
            {
                var cellPos = _board.GetPosition(blockView.Block);
                var worldPos = _boardView.CellToWorld(new(cellPos.Item1, cellPos.Item2));
                // moveTask.Add(blockView.Move(worldPos, duration));
                blockView.Move(worldPos, duration);
            }
            // await Task.WhenAll(moveTask);
            yield return new WaitForSeconds(duration);
        }

        public void GenerateBoard()
        {
            _boardView.Clear();
            
            BoardGenerator.GenerateBoard(_board);
            // Debug.Log($"Board has {_board.BlockDict.Count(d => d.Value != null)} blocks");

            foreach(var pos in _board.BlockDict.Keys)
            {
                SpawnBlockView(_board.BlockDict[pos], pos);
                // await newBlock.Move(worldPos, 0f);
            }

            OnUpdateBoard?.Invoke();
            // Debug.Log($"Board view has {_boardView.GetAllBlocks().Count(b => b.gameObject.activeInHierarchy)} blocks");
        }

        public IEnumerator Suffle(float duration)
        {
            Debug.Log("Start Suffle");
            while (!BoardMatcher.HasValidMove(_board)) 
                BoardSwapper.Suffle(_board);
            yield return StartCoroutine(SyncBoard(duration));

            OnUpdateBoard?.Invoke();
        }

        public IEnumerator Swap(IngameBlockView block1, IngameBlockView block2)
        // public async Task Swap(IngameBlockView block1, IngameBlockView block2)
        {
            AudioControl.Instance.PlaySfx(_swapClip);

            var worldPos1 = block1.transform.position;
            var worldPos2 = block2.transform.position;
            var cellPos1 = _boardView.WorldToCell(worldPos1);
            var cellPos2 = _boardView.WorldToCell(worldPos2);

            BoardSwapper.Swap(_board, (cellPos1.x, cellPos1.y), (cellPos2.x, cellPos2.y));
            // await Task.WhenAll(block1.Move(worldPos2, .2f), block2.Move(worldPos1, .2f));
            block1.Move(worldPos2, .2f);
            block2.Move(worldPos1, .2f);
            yield return new WaitForSeconds(.2f);
        }

        public IEnumerator HandleSwapBlocks(IngameBlockView blockToSwap)
        {
            if (SelectedBlock == null) yield break;

            _canInput = false;
            yield return Swap(SelectedBlock, blockToSwap);

            var match1 = BoardMatcher.HasMatchAt(_board, blockToSwap.Block, out var matchedHor1, out var matchedVer1);
            var match2 = BoardMatcher.HasMatchAt(_board, SelectedBlock.Block, out var matchedHor2, out var matchedVer2);
            var abilityMatch = (SelectedBlock.Block.AbilityType != EAbilityType.None && blockToSwap.Block.AbilityType != EAbilityType.None)
                               || SelectedBlock.Block.AbilityType == EAbilityType.ClearInstance
                               || blockToSwap.Block.AbilityType == EAbilityType.ClearInstance;
            
            bool validMove = abilityMatch || match1 || match2;
            if (abilityMatch)
            {
                // Activate both ability
                HandleAbilityMatch(SelectedBlock.Block, blockToSwap.Block);
                yield return ApplyGravity();
            }
            else if (!match1 && !match2)
            {
                // Return position
                yield return Swap(SelectedBlock, blockToSwap);
            }
            else
            {
                // Normal match
                if (match1) HandleMatch(matchedHor1, matchedVer1);
                if (match2) HandleMatch(matchedHor2, matchedVer2);
                yield return ApplyGravity();
            }

            if (SelectedBlock != null) SelectedBlock.Deselect();
            SelectedBlock = null;
            _selectBorder.gameObject.SetActive(false);

            // Clean Match
            while (BoardMatcher.HasMatch(_board, out var allMatched))
            {
                foreach(var matched in allMatched)
                {
                    HandleMatch(matched.Item1, matched.Item2);
                }

                yield return ApplyGravity();

                // Check have valid move
                if (!BoardMatcher.HasValidMove(_board))
                {
                    yield return Suffle(.2f);
                }
            }

            // // Clean matchs and suffle if no valid move
            yield return CleanMatch();
            _canInput = true;

            if (validMove) OnMove?.Invoke();
        }

        private IEnumerator CleanMatch()
        {
            while (BoardMatcher.HasMatch(_board, out var allMatched))
            {
                foreach(var matched in allMatched)
                {
                    HandleMatch(matched.Item1, matched.Item2);
                }

                yield return ApplyGravity();
            }
            
            // Check have valid move
            if (!BoardMatcher.HasValidMove(_board))
            {
                yield return Suffle(.2f);
                yield return CleanMatch();
            }
        }

        public void ClearBoard()
        {
            _board.Clear();
            _boardView.Clear();
        }
        #endregion

        #region Initialize
        public void InitBoard(LevelRuntimeData levelRuntimeData)
        {
            var bounds = _boardView.GetBounds();
            var tiledPos = _boardView.GetTiledPositions();
            _board = new(bounds.min, bounds.max, tiledPos);

            GenerateBoard();
        }        
        
        public BoardModel GetModel() => _board;
        #endregion

        protected override void Awake()
        {
            _hoverBorder.gameObject.SetActive(false);
            _selectBorder.gameObject.SetActive(false);
            
            _boardView = FindFirstObjectByType<BoardView>();
        }

        void Start()
        {
            // var bounds = _boardView.GetBounds();
            // var tiledPos = _boardView.GetTiledPositions();
            // _board = new(bounds.min, bounds.max, tiledPos);
            // Debug.Log($"Tile size: {_boardView.GetTileSize()}");

            // GenerateBoard();
        }

        void Update()
        {
            if (_board == null) return;

            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            RaycastHit2D hit = Physics2D.Raycast(
                mouseWorld,
                Vector2.zero,
                Mathf.Infinity,
                LayerMask.GetMask("Board", "Block")
            );

            if (hit.collider == null)
            {
                _hoverBorder.gameObject.SetActive(false);
            }
            else if (hit.collider.gameObject.TryGetComponent<IngameBlockView>(out var block))
            {
                _hoverBorder.gameObject.SetActive(true);
                _hoverBorder.gameObject.transform.position = block.transform.position;
            }
        }
    }
}
