﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using sm_application.Events;
using sm_application.Extension;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Weapon
{
    public class Destruction : MonoBehaviour
    {
        [SerializeField] private Behaviors _behavior;
        [SerializeField] private float _lifeTime = 5f;

        private Transform[] _childrenTransforms;
        private TransformInfo[] _childrenInitTransformInfos;
        private CancellationToken _destroyCancellationToken;

        private void Awake()
        {
            _destroyCancellationToken = gameObject.GetCancellationTokenOnDestroy();
            _childrenTransforms = transform.GetChildrenTransforms();
            _childrenInitTransformInfos = transform.GetChildrenTransformInfo();
        }

        private void OnEnable()
        {
            RestoreTransforms();

            switch (_behavior)
            {
                case Behaviors.ReturnToPool:
                    ReturnToPoolByLifetime();
                    break;
                case Behaviors.FadeAlpha:
                    throw new NotImplementedException();
                case Behaviors.FadeScaleChildren:
                    FadeScaleChildren();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RestoreTransforms()
        {
            for (var i = 0; i < _childrenTransforms.Length; i++)
            {
                var childTransform = _childrenTransforms[i];
                var childInitTransformInfo = _childrenInitTransformInfos[i];
                childTransform.localPosition = childInitTransformInfo.LocalPosition;
                childTransform.localRotation = childInitTransformInfo.LocalRotation;
                childTransform.localScale = childInitTransformInfo.LocalScale;
            }
        }

        private async void ReturnToPoolByLifetime()
        {
            await _lifeTime.WaitInSeconds();
            new RequestReturnToPool() { Instance = gameObject }.Fire();
        }

        private void FadeScaleChildren()
        {
            var children = transform.GetChildrenTransforms();
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(_lifeTime / 2f);
            sequence.OnComplete(() => new RequestReturnToPool() { Instance = gameObject }.Fire());

            foreach (var child in children)
            {
                sequence.Join(child.DOScale(0f, _lifeTime / 2f * Random.Range(0.5f, 1f))).WithCancellation(child.gameObject.GetCancellationTokenOnDestroy());
            }

            sequence.Play()
                .ToUniTaskWithCancel(TweenCancelBehaviour.KillAndCancelAwait, _destroyCancellationToken)
                .Forget();
        }

        private enum Behaviors
        {
            ReturnToPool,
            FadeAlpha,
            FadeScaleChildren,
        }
    }
}