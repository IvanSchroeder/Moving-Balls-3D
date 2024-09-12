using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class SlotsManager : MonoBehaviour
{
    [SerializeField] private List<BallType_SO> BallTypes = new List<BallType_SO>();
    
    [SerializeField] private List<GameObject> SlotsGOList = new List<GameObject>();
    [SerializeField] private List<Slot> SlotsList = new List<Slot>();
    [SerializeField] private List<Ball> SortedBallsList = new List<Ball>();
    [SerializeField] private List<Ball> ShuffledBallsList = new List<Ball>();

    [SerializeField] private List<Slot> LeftSlotsList = new List<Slot>();
    [SerializeField] private List<Slot> RightSlotsList = new List<Slot>();
    [SerializeField] private List<Slot> MiddleSlotsList = new List<Slot>();

    private List<Slot> TempSlotsList = new List<Slot>();
    [SerializeField] private List<Ball> TempBallsList = new List<Ball>();

    [SerializeField] private LevelData levelData;
    private BallType_SO firstSelectedBallType;
    private BallType_SO secondSelectedBallType;
    private int countPrimaryBallType;
    private int countSecondaryBallType;
    private int currentBallCount;

    private int maxAmountOfBalls;

    private int maxType, minType, diffInTypes;

    private bool isSorted = false;

    [SerializeField] private bool isFirstSided = true;

    [SerializeField] private int swipesCount;
    [SerializeField] private int tapCount;
    [SerializeField] private int firstMoveFlag;

    public static event Action OnBallsSorted;
    public static event Action OnFirstTap;
    public static event Action OnFirstSwipe;
    public static event Action OnFirstMove;
    public static event Action<BallType_SO, BallType_SO> OnColorsSelected;

    public static event Action OnSlotsCreated;

    private void Start() {
        OnSlotsCreated?.Invoke();
        Initialize();
    }

    private void OnEnable() {
        //LevelManager.OnLevelStart += SetLevelData;

        InputManager.OnAnchorPointChanged += ChangeSide;
        InputManager.OnAnchored += CheckWinCondition;

        InputManager.OnLeftSwipe += LeftSwipeRotation;
        InputManager.OnRightSwipe += RightSwipeRotation;

        OnBallsSorted += HideAllBalls;
    }

    private void OnDisable() {
        //LevelManager.OnLevelStart -= SetLevelData;

        InputManager.OnAnchorPointChanged -= ChangeSide;
        InputManager.OnAnchored -= CheckWinCondition;

        InputManager.OnLeftSwipe -= LeftSwipeRotation;
        InputManager.OnRightSwipe -= RightSwipeRotation;

        OnBallsSorted -= HideAllBalls;
    }

    private void LeftSwipeRotation() {
        if (isFirstSided) {
            RotateBallsToPrevious(LeftSlotsList);
        }
        else {
            RotateBallsToPrevious(RightSlotsList);
        }
    }

    private void RightSwipeRotation() {
        if (isFirstSided) {
            RotateBallsToNext(LeftSlotsList);
        }
        else {
            RotateBallsToNext(RightSlotsList);
        }
    }

    private void RotateBallsToNext(List<Slot> SideList) {
        TempBallsList = new List<Ball>();

        foreach (Slot slot in SideList) {
            int index = SideList.IndexOf(slot);
            Ball storedBall = slot._storedBall;

            TempBallsList.Add(slot._storedBall);
        }

        Ball lastBall = TempBallsList[TempBallsList.Count - 1];
        TempBallsList.RemoveAt(TempBallsList.Count - 1);
        TempBallsList.Insert(0, lastBall);

        foreach (Ball ball in TempBallsList) {
            int index = TempBallsList.IndexOf(ball);
            Slot referenceSlot = SideList[index];

            referenceSlot._storedBall = ball;
            referenceSlot._slotPosition = referenceSlot.transform.position;
            referenceSlot._storedBall.transform.parent = referenceSlot.transform;
            referenceSlot._storedBall.SetRotationState(true, referenceSlot._slotPosition);
        }

        CheckFirstSwipe();
    }

    private void RotateBallsToPrevious(List<Slot> SideList) {
        TempBallsList = new List<Ball>();

        foreach (Slot slot in SideList) {
            int index = SideList.IndexOf(slot);
            Ball storedBall = slot._storedBall;

            TempBallsList.Add(slot._storedBall);
        }

        Ball firstBall = TempBallsList[0];
        TempBallsList.RemoveAt(0);
        TempBallsList.Add(firstBall);

        foreach (Ball ball in TempBallsList) {
            int index = TempBallsList.IndexOf(ball);
            Slot referenceSlot = SideList[index];

            referenceSlot._storedBall = ball;
            referenceSlot._slotPosition = referenceSlot.transform.position;
            referenceSlot._storedBall.transform.parent = referenceSlot.transform;
            referenceSlot._storedBall.SetRotationState(true, referenceSlot._slotPosition);
        }

        CheckFirstSwipe();
    }

    private void CheckFirstTap() {
        CheckFirstMove();
        if (tapCount == 0) {
            tapCount++;
            OnFirstTap?.Invoke();
        }
    }

    private void CheckFirstSwipe() {
        CheckFirstMove();
        if (swipesCount == 0) {
            swipesCount++;
            OnFirstSwipe?.Invoke();
        }
    }

    private void CheckFirstMove() {
        if (firstMoveFlag == 0) {
            firstMoveFlag++;
            OnFirstMove?.Invoke();
        }
    }

    public void ChangeSide(bool _isFirstSided) {
        isFirstSided = _isFirstSided;

        CheckFirstTap();

        foreach (Slot slot in SlotsList) {
            slot._slotPosition = slot._slotGOReference.transform.position;
            slot._storedBall.transform.parent = slot._slotGOReference.transform;
            slot._storedBall.transform.position = slot._slotPosition;
        }

        if (isFirstSided) {

            foreach(Slot slot in MiddleSlotsList) {
                LeftSlotsList.Add(slot);
                RightSlotsList.Remove(slot);
            }

            LoopInList(LeftSlotsList, true);
            LoopInList(RightSlotsList);
        }
        else {

            foreach(Slot slot in MiddleSlotsList) {
                LeftSlotsList.Remove(slot);
                RightSlotsList.Add(slot);
            }

            LoopInList(LeftSlotsList);
            LoopInList(RightSlotsList, true);
        }
        LoopInList(MiddleSlotsList, true);
    }

    private void Initialize() {
        ResetCount();
        SelectBallTypes();
        UpdateSlotList();
        CreateBallsList();
        ShuffleBallsList(3);
        AllocateBalls(ShuffledBallsList);
        SetInitialSlotsChain();
    }

    private void ResetCount() {
        swipesCount = 0;
        countPrimaryBallType = 0;
        countSecondaryBallType = 0;
        currentBallCount = 0;

        maxAmountOfBalls = Sum(levelData.maxFirstBallType, levelData.maxSecondBallType);
        maxType = Mathf.Max(levelData.maxFirstBallType, levelData.maxSecondBallType);
        minType = Mathf.Min(levelData.maxFirstBallType, levelData.maxSecondBallType);
        diffInTypes = Difference(maxType, minType);
    }

    private void SelectBallTypes() {
        firstSelectedBallType = levelData.firstBallType;
        secondSelectedBallType = levelData.secondBallType;
        
        if (levelData.isSideDependant) OnColorsSelected?.Invoke(firstSelectedBallType, secondSelectedBallType);
    }

    private void SelectRandomBallTypes() {
        if (levelData.isRandom) {
            firstSelectedBallType = BallTypes[Random.Range(0, BallTypes.Count)];
            secondSelectedBallType = BallTypes[Random.Range(0, BallTypes.Count)];
            
            while (secondSelectedBallType == firstSelectedBallType) {
                secondSelectedBallType = BallTypes[Random.Range(0, BallTypes.Count)];
            }
        }
        else {
            firstSelectedBallType = levelData.firstBallType;
            secondSelectedBallType = levelData.secondBallType;
        }

        if (levelData.isSideDependant) OnColorsSelected?.Invoke(firstSelectedBallType, secondSelectedBallType);
    }

    private void UpdateSlotList() {
        foreach (GameObject slot in SlotsGOList) {
            int index = SlotsGOList.IndexOf(slot);

            Slot slotComponent = slot.GetComponent<Slot>();
            Slot temp = slotComponent.InitializeSlot(slotComponent, slot.name, slot, slot.transform.position, false);
            SlotsList[index] = temp;
        }

        int i = 0;
        int j = 0;
        int k = 0;

        for (i = 0; i < minType; i++) {
            LeftSlotsList.Add(SlotsList[i]);
        }

        int diffInBalls = Difference(maxAmountOfBalls, diffInTypes);

        for (j = j + i; j < diffInBalls; j++) {
            RightSlotsList.Add(SlotsList[j]);
        }

        for (k = k + j; k < maxAmountOfBalls; k++) {
            MiddleSlotsList.Add(SlotsList[k]);
        }
    }

    private void CreateBallsList() {
        foreach (Slot slot in SlotsList) {

            if (currentBallCount < levelData.maxFirstBallType) {
                CreateBall(firstSelectedBallType, slot._slotGOReference, countPrimaryBallType);
                countPrimaryBallType++;
            }
            else if (currentBallCount < maxAmountOfBalls) {
                CreateBall(secondSelectedBallType, slot._slotGOReference, countSecondaryBallType);
                countSecondaryBallType++;
            }

            currentBallCount++;
        }
    }

    private void ShuffleBallsList(int repetitions) {
        ShuffledBallsList = new List<Ball>(SortedBallsList);
        
        for (int j = 0; j < repetitions; j++) {
            for (int i = 0; i < ShuffledBallsList.Count; i++) {
                Ball temp = ShuffledBallsList[i];
                int randomIndex = Random.Range(i, ShuffledBallsList.Count);
                ShuffledBallsList[i] = ShuffledBallsList[randomIndex];
                ShuffledBallsList[randomIndex] = temp;
            }
        }
    }

    private void AllocateBalls(List<Ball> _ballList) {
        int j = 0;

        foreach (Slot slot in SlotsList) {
            Ball ball = _ballList[j];
            ball.transform.position = slot._slotPosition;
            ball.transform.parent = slot._slotGOReference.transform;
            slot._storedBall = ball;
            j++;
        }
    }

    private void SetInitialSlotsChain() {
        LoopInList(LeftSlotsList, true);
        LoopInList(RightSlotsList);
        LoopInList(MiddleSlotsList, true);

        foreach(Slot slot in MiddleSlotsList) {
            LeftSlotsList.Add(slot);
            RightSlotsList.Remove(slot);
        }
    }

    private void LoopInList(List<Slot> List, bool canRotate = false) {
        foreach (Slot slot in List) {
            int index = List.IndexOf(slot);
            
            if (List != MiddleSlotsList) {
                if (index == 0) {
                    slot._previousSlot = MiddleSlotsList[MiddleSlotsList.Count - 1];
                    slot._nextSlot = List[index + 1];
                }
                else if (index < List.Count - 1) {
                    slot._previousSlot = List[index - 1];
                    slot._nextSlot = List[index + 1];
                }
                else if (index == List.Count - 1) {
                    slot._previousSlot = List[index - 1];
                    slot._nextSlot = MiddleSlotsList[0];
                }
            }
            else {
                if (levelData.middleSlotsAmount == 1) {
                    if (isFirstSided) {
                        slot._previousSlot = LeftSlotsList[LeftSlotsList.Count - 1];
                        slot._nextSlot = LeftSlotsList[0];
                    }
                    else {
                        slot._previousSlot = RightSlotsList[LeftSlotsList.Count - 1];
                        slot._nextSlot = RightSlotsList[0];
                    }
                }
                else {
                    if (index == 0) {
                        if (isFirstSided) slot._previousSlot = LeftSlotsList[LeftSlotsList.Count - 1];
                        else slot._previousSlot = RightSlotsList[RightSlotsList.Count - 1];
    
                        slot._nextSlot = MiddleSlotsList[index + 1];
                    }
                    else if (index < List.Count - 1) {
                        slot._previousSlot = MiddleSlotsList[index - 1];
                        slot._nextSlot = MiddleSlotsList[index + 1];
                    }
                    else if (index == List.Count - 1) {
                        slot._previousSlot = MiddleSlotsList[index - 1];
                        if (isFirstSided) slot._nextSlot = LeftSlotsList[0];
    
                        else slot._nextSlot = RightSlotsList[0];
                    }
                }
            }
            slot._canRotate = canRotate;
        }
    }

    private void CheckWinCondition() {
        if (levelData.isSideDependant) {
            DependantCheck();
        }
        else {
            IndependantCheck();
        }
    }

    private void DependantCheck() {
        bool firstSideSorted = false;
        bool secondSideSorted = false;

        int idCountFirst = 0;
        int idCountSecond = 0;

        foreach (Slot slot in LeftSlotsList) {
            if (slot._storedBall.ballType == firstSelectedBallType) {
                idCountFirst++;
            }
        }

        foreach (Slot slot in RightSlotsList) {
            if (slot._storedBall.ballType == secondSelectedBallType) {
                idCountSecond++;
            }
        }

        if (idCountFirst == levelData.maxFirstBallType) {
            firstSideSorted = true;
        }
        if (idCountSecond == levelData.maxSecondBallType) {
            secondSideSorted = true;
        }

        if (firstSideSorted && secondSideSorted) {
            isSorted = true;
        }
        else {
            isSorted = false;
        }

        if (isSorted) {
            OnBallsSorted?.Invoke();
        }
    }

    private void IndependantCheck() {
        BallType_SO maxTypeData = maxType == levelData.maxFirstBallType ? firstSelectedBallType : secondSelectedBallType;

        int winCheck = 0;

        for (int i = 0; i < 2; i++) {
            switch (i) {
                case 0:
                    int idCountLeft = 0;

                    foreach (Slot slot in LeftSlotsList) {
                        int _ballID = slot._storedBall.ballType.ballID;

                        if (_ballID == maxTypeData.ballID) {
                            idCountLeft++;
                        }
                    }

                    if (idCountLeft == maxType) winCheck++;
                break;

                case 1:
                    int idCountRight = 0;

                    foreach (Slot slot in RightSlotsList) {
                        int _ballID = slot._storedBall.ballType.ballID;

                        if (_ballID == maxTypeData.ballID) {
                            idCountRight++;
                        }
                    }

                    if (idCountRight == maxType) winCheck++;

                break;
            }
        }

        if (winCheck == 1) isSorted= true;
        else isSorted = false;

        if (isSorted) {
            OnBallsSorted?.Invoke();
        }
    }

    public void CreateBall(BallType_SO ballType, GameObject parentSlot, int ballCount) {
        GameObject ballGO = Instantiate(ballType.ballPrefab, parentSlot.transform.position, Quaternion.identity);
        Ball ballComponent = ballGO.GetComponent<Ball>();
        SortedBallsList.Add(ballComponent);
        ballComponent.ballType = ballType;

        ballGO.transform.parent = parentSlot.transform;
        ballGO.GetComponent<MeshRenderer>().material.color = ballType.color;
        ballGO.name = ballType.ballColor + "Ball_" + ballCount;
    }

    private void HideAllBalls() {
        foreach (Ball ball in SortedBallsList) {
            LeanTween.moveY(ball.gameObject, ball.gameObject.transform.position.y - 3f, 1f).setEaseInBack().setDelay(0.5f);
            LeanTween.scale(ball.gameObject, Vector3.zero, 1f).setEaseInBack().setDelay(0.5f);
        }
    }

    /*private void SetLevelData(LevelData _currentLevel) {
        levelData = _currentLevel;
        Debug.Log("Level data set " + levelData);
    }*/

    private int Sum(int a, int b) {
        return a + b;
    }

    private int Difference(int a, int b) {
        return a - b;
    }
}