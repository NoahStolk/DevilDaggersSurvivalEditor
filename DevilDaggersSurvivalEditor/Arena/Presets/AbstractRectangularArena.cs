using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public abstract class AbstractRectangularArena : AbstractArena
{
	private int _x1 = 15;
	private int _y1 = 15;
	private int _x2;
	private int _y2;

	protected AbstractRectangularArena(int dimension)
		: base(dimension)
	{
		_x2 = dimension - 15;
		_y2 = dimension - 15;
	}

	public int X1 { get => _x1; set => _x1 = Math.Clamp(value, 0, Dimension); }

	public int Y1 { get => _y1; set => _y1 = Math.Clamp(value, 0, Dimension); }

	public int X2 { get => _x2; set => _x2 = Math.Clamp(value, 0, Dimension); }

	public int Y2 { get => _y2; set => _y2 = Math.Clamp(value, 0, Dimension); }

	public override bool IsFull => false;
}
