using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] mapPrefab = new GameObject[3];
    [SerializeField] private GameObject playerMap = null;
    [SerializeField] private GameObject challMap = null;
    [SerializeField] int idx;

    BrickListManager brickListManager1;
    BrickListManager brickListManager2;

    // MapSelectManager에서 가져온 값으로 랜덤한 idx 설정
    private void Awake()
    {
        idx = MapSelectManager.Instance.GetMapSelect();
    }
    private void OnEnable()
    {
        SetingMap();
    }

    // idx에 저장된 랜덤한 맵을 지정한 position에 위치시켜 호출
    // 맵의 Brick HP를 저장시킬 BrickListManager 호출
    public void SetingMap()
    {
        playerMap = Instantiate(mapPrefab[idx], transform.position + new Vector3(-9.5f, -1.5f, 0), Quaternion.identity);
        brickListManager1 = playerMap.GetComponent<BrickListManager>();
        brickListManager1.hpManager();

        challMap = Instantiate(mapPrefab[idx], transform.position + new Vector3(-0.5f, -1.5f, 0), Quaternion.identity);
        brickListManager2 = challMap.GetComponent<BrickListManager>();
        brickListManager2.hpManager();
    }
}
