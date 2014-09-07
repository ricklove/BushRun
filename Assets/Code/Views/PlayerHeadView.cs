using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerHeadView
{
    public HeadType HeadType { get; set; }

    private PlayerData _playerData = null;
    private Dictionary<HeadType, List<Sprite>> _heads = new Dictionary<HeadType, List<Sprite>>();
    private SpriteRenderer _headRenderer;

    private float _nextChangeTime = 0;
    private int _iHead = 0;

    public void Initialize(MonoBehaviour owner)
    {
        if (_headRenderer == null)
        {
            _headRenderer = owner.GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void Update(IPlayerViewModel playerViewModel)
    {
        if (playerViewModel.PlayerData != _playerData)
        {
            _playerData = playerViewModel.PlayerData;

            _heads[HeadType.Idle] = _playerData.Sprites.Where(s => s.SpriteType == SpriteType.HeadIdle).Select(s => s.Sprite).ToList();
            _heads[HeadType.Happy] = _playerData.Sprites.Where(s => s.SpriteType == SpriteType.HeadHappy).Select(s => s.Sprite).ToList();
            _heads[HeadType.Hurt] = _playerData.Sprites.Where(s => s.SpriteType == SpriteType.HeadHurt).Select(s => s.Sprite).ToList();
        }

        if (Time.time > _nextChangeTime)
        {
            _nextChangeTime = Random.Range(Time.time, Time.time + 10);

            // Show heads
            var heads = _heads[HeadType];

            if (heads == null
                || heads.Count <= 0)
            {
                heads = _heads[HeadType.Idle];
            }

            if (heads == null)
            {
                return;
            }

            _iHead++;
            if (_iHead >= heads.Count)
            {
                _iHead = 0;
            }

            _headRenderer.sprite = heads[_iHead];
        }
    }
}

public enum HeadType
{
    Idle,
    Happy,
    Hurt
}
