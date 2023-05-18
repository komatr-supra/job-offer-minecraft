using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simply drop, interface should be better, but no needed here
public class Drop : MonoBehaviour
{
    public BlocksSO BlocksSO;
    public void Init(BlocksSO blocksSO)
    {
        this.BlocksSO = blocksSO;
        //set visual
    }
}
