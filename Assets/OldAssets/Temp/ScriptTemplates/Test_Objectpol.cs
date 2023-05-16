using _Project.Scripts.Extension;
using _Project.Scripts.Main.Game.Weapon;
using _Project.Scripts.Main.Wrappers;
using UnityEngine;
using UnityEngine.Pool;

public class Test_Objectpol : MonoBehaviour
{
    [SerializeField] private ObjectPool<ShellBase> _objectPool;

    private void Start()
    {
        var c = new Controls();
        c.Enable();

        for (int i = 0; i < 20; i++)
        {
            var t = _objectPool.Get();
           // t.SetActive(true);
        }
        
        c.Player.Move.BindAction(BindActions.Started, (ctx) =>
        {
            var t = _objectPool.Get();
           // t.SetActive(true);
        });
    }

}
