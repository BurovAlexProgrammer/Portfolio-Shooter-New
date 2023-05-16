using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

[Serializable]
public class TestClass
{
    public int t;
}

[Serializable]
public class Gen<T>
{
    public T Value;
}

public class TestDoTween : MonoBehaviour
{
    [SerializeField] RangeInt _rn;
    [SerializeField] int temp;
    [SerializeField] private TestClass _testClass;
    public Gen<int> gen;
    private Sequence _sequence;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Play();
        }
    }

    void Play()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence
            .OnComplete(() => Debug.Log("Complete"));
        Foo();
    }

    void Foo()
    {
        _sequence
            .AppendInterval(2)
            .AppendCallback(() => Debug.Log("After 2 sec"));
    }
}
