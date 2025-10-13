using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class BoomFireBlockOne : Node2D
{
	private Area2D _attackArea;
	private System.Collections.Generic.List<AnimatedSprite2D> _sprites = new System.Collections.Generic.List<AnimatedSprite2D>();
	private float _elapsed = 0f;
	private bool _destroyed = false;

	public override void _Ready()
	{
		_attackArea = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
		if (_attackArea != null)
		{
			_attackArea.AreaEntered += OnAttackAreaEntered;
		}
		// 扫描所有子孙节点中的AnimatedSprite2D
		FindAllAnimatedSprites(this);
	}

	private void FindAllAnimatedSprites(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is AnimatedSprite2D sprite)
				_sprites.Add(sprite);
			FindAllAnimatedSprites(child);
		}
	}

    int damage = BulletConstants.DamageLaJiao;
	private void OnAttackAreaEntered(Area2D area)
	{
		// TODO: 这里可以添加碰撞逻辑
        // TODO: 实际攻击逻辑
        IHurtBase hurtBase = area as IHurtBase;
        if (hurtBase != null)
        {
            hurtBase.TakeDamage(EnumObjType.Plans, damage, EnumHurts.Boom);
        }
	}

	public override void _Process(double delta)
	{
		if (_destroyed) return;
		_elapsed += (float)delta;
		float t = Mathf.Min(_elapsed / 0.5f, 1f);
		float moveY = 120f * t;
		float alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp((_elapsed - 0.1f) / 0.5f, 0f, 1f));
		foreach (var sprite in _sprites)
		{
			var pos = sprite.Position;
			sprite.Position = new Vector2(pos.X, pos.Y - moveY * (float)delta);
			var mod = sprite.Modulate;
			sprite.Modulate = new Color(mod.R, mod.G, mod.B, alpha);
		}
		if (_elapsed >= 0.6f)
		{
			_destroyed = true;
			QueueFree();
		}
	}
}
