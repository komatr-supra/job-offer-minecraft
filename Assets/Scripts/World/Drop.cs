using DG.Tweening;
using UnityEngine;

//simply drop, interface should be better, but no needed here
public class Drop : MonoBehaviour
{
    //simple block data its for inventory too(some type of getter should be better)
    public BlocksSO BlocksSO;
    private void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 2, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart);        
    }
    public void Init(BlocksSO blocksSO)
    {
        this.BlocksSO = blocksSO;
        GetComponent<MeshRenderer>().material = blocksSO.material;
    }
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
