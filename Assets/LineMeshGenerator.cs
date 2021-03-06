using System.Collections;
using System.Collections.Generic;
using System;



public static class LineMeshGenerator
{
	public struct Vertex
	{
		public float x;

		public float y;

		public Vertex normalize
		{
			get
			{
				if( Math.Abs( x ) > Math.Abs( y ) )
				{
					return new Vertex( x / Math.Abs( x ), y / Math.Abs( x ) );
				}
				else
				{
					return new Vertex( x / Math.Abs( y ), y / Math.Abs( y ) );
				}
			}
		}

		public Vertex( float x, float y )
		{
			this.x = x;
			this.y = y;
		}

		public static Vertex operator +( Vertex left, Vertex right )
		{
			return new Vertex( left.x + right.x, left.y + right.y );
		}

		public static Vertex operator -( Vertex left, Vertex right )
		{
			return new Vertex( left.x - right.x, left.y - right.y );
		}
	}

	public static List<Vertex> Generate( List<Vertex> path )
	{
		List<Vertex> points = new List<Vertex>();
		for( int i = 0; i < path.Count - 1; i++ )
		{
			var dir = path[i + 1] - path[i];
			dir = dir.normalize;
			var vertical = GetVerticalDir( dir );
			var vert1 = new Vertex( path[i].x, path[i].y );
			var vert2 = new Vertex( path[i].x + vertical.x, path[i].y + vertical.y );
			var vert3 = new Vertex( path[i + 1].x, path[i + 1].y );
			var vert4 = new Vertex( path[i + 1].x + vertical.x, path[i + 1].y + vertical.y );
			points.Add( vert1 );
			points.Add( vert2 );
			points.Add( vert3 );
			points.Add( vert4 );
		}
		return points;
	}

	private static Vertex GetVerticalDir( Vertex dir )
	{
		var a = dir.x;
		var b = dir.y;
		var x = (float)( -b * Math.Sqrt( 1 / ( b * b + a * a ) ) );
		var y = (float)Math.Sqrt( a * a / ( a * a + b * b ) );
		return new Vertex( x, y );
	}
}
