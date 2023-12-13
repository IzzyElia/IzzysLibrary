
namespace Izzy.Geometry
{
	public static class MathG
	{
		public static Vector RadiansToVector (float radians)
		{
			return new Vector((float)System.Math.Cos(radians), (float)System.Math.Sin(radians));
		}
		/// <summary>
		/// Returns a random point in a circle with a diameter of 1
		/// </summary>
		public static Vector RandomPointInACircle (int seed)
		{
			float angle = Random.Float(seed) * 2 * Mathfi.PI;
			float distance = 0.5f * Mathfi.Sqrt(Random.Float(seed << 16));
			return new Vector
				(
				distance * Mathfi.Cos(angle),
				distance * Mathfi.Sin(angle)
				);
		}
	}
}
