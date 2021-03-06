using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Used for custom and animationCurve-based ease functions. Must return a value between 0 and 1.
/// </summary>
public delegate float EaseFunction( float time, float duration, float overshootOrAmplitude, float period );

public enum Ease
{
	Unset, // Used to let TweenParams know that the ease was not set and apply it differently if used on Tweeners or Sequences
	Linear,
	InSine,
	OutSine,
	InOutSine,
	InQuad,
	OutQuad,
	InOutQuad,
	InCubic,
	OutCubic,
	InOutCubic,
	InQuart,
	OutQuart,
	InOutQuart,
	InQuint,
	OutQuint,
	InOutQuint,
	InExpo,
	OutExpo,
	InOutExpo,
	InCirc,
	OutCirc,
	InOutCirc,
	InElastic,
	OutElastic,
	InOutElastic,
	InBack,
	OutBack,
	InOutBack,
	InBounce,
	OutBounce,
	InOutBounce,
	// Extra custom eases
	Flash, InFlash, OutFlash, InOutFlash,
	/// <summary>
	/// Don't assign this! It's assigned automatically when creating 0 duration tweens
	/// </summary>
	INTERNAL_Zero,
	/// <summary>
	/// Don't assign this! It's assigned automatically when setting the ease to an AnimationCurve or to a custom ease function
	/// </summary>
	INTERNAL_Custom
}
public static class EaseManager
{
	const float _PiOver2 = Mathf.PI * 0.5f;
	const float _TwoPi = Mathf.PI * 2;

	/// <summary>
	/// Returns a value between 0 and 1 (inclusive) based on the elapsed time and ease selected
	/// </summary>
	public static float Evaluate( Ease easeType, EaseFunction customEase, float time, float duration, float overshootOrAmplitude, float period )
	{
		switch( easeType )
		{
			case Ease.Linear:
				return time / duration;
			case Ease.InSine:
				return -(float)Math.Cos( time / duration * _PiOver2 ) + 1;
			case Ease.OutSine:
				return (float)Math.Sin( time / duration * _PiOver2 );
			case Ease.InOutSine:
				return -0.5f * ( (float)Math.Cos( Mathf.PI * time / duration ) - 1 );
			case Ease.InQuad:
				return ( time /= duration ) * time;
			case Ease.OutQuad:
				return -( time /= duration ) * ( time - 2 );
			case Ease.InOutQuad:
				if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time;
				return -0.5f * ( ( --time ) * ( time - 2 ) - 1 );
			case Ease.InCubic:
				return ( time /= duration ) * time * time;
			case Ease.OutCubic:
				return ( ( time = time / duration - 1 ) * time * time + 1 );
			case Ease.InOutCubic:
				if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time * time;
				return 0.5f * ( ( time -= 2 ) * time * time + 2 );
			case Ease.InQuart:
				return ( time /= duration ) * time * time * time;
			case Ease.OutQuart:
				return -( ( time = time / duration - 1 ) * time * time * time - 1 );
			case Ease.InOutQuart:
				if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time * time * time;
				return -0.5f * ( ( time -= 2 ) * time * time * time - 2 );
			case Ease.InQuint:
				return ( time /= duration ) * time * time * time * time;
			case Ease.OutQuint:
				return ( ( time = time / duration - 1 ) * time * time * time * time + 1 );
			case Ease.InOutQuint:
				if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time * time * time * time;
				return 0.5f * ( ( time -= 2 ) * time * time * time * time + 2 );
			case Ease.InExpo:
				return ( time == 0 ) ? 0 : (float)Math.Pow( 2, 10 * ( time / duration - 1 ) );
			case Ease.OutExpo:
				if( time == duration ) return 1;
				return ( -(float)Math.Pow( 2, -10 * time / duration ) + 1 );
			case Ease.InOutExpo:
				if( time == 0 ) return 0;
				if( time == duration ) return 1;
				if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * (float)Math.Pow( 2, 10 * ( time - 1 ) );
				return 0.5f * ( -(float)Math.Pow( 2, -10 * --time ) + 2 );
			case Ease.InCirc:
				return -( (float)Math.Sqrt( 1 - ( time /= duration ) * time ) - 1 );
			case Ease.OutCirc:
				return (float)Math.Sqrt( 1 - ( time = time / duration - 1 ) * time );
			case Ease.InOutCirc:
				if( ( time /= duration * 0.5f ) < 1 ) return -0.5f * ( (float)Math.Sqrt( 1 - time * time ) - 1 );
				return 0.5f * ( (float)Math.Sqrt( 1 - ( time -= 2 ) * time ) + 1 );
			case Ease.InElastic:
				float s0;
				if( time == 0 ) return 0;
				if( ( time /= duration ) == 1 ) return 1;
				if( period == 0 ) period = duration * 0.3f;
				if( overshootOrAmplitude < 1 )
				{
					overshootOrAmplitude = 1;
					s0 = period / 4;
				}
				else s0 = period / _TwoPi * (float)Math.Asin( 1 / overshootOrAmplitude );
				return -( overshootOrAmplitude * (float)Math.Pow( 2, 10 * ( time -= 1 ) ) * (float)Math.Sin( ( time * duration - s0 ) * _TwoPi / period ) );
			case Ease.OutElastic:
				float s1;
				if( time == 0 ) return 0;
				if( ( time /= duration ) == 1 ) return 1;
				if( period == 0 ) period = duration * 0.3f;
				if( overshootOrAmplitude < 1 )
				{
					overshootOrAmplitude = 1;
					s1 = period / 4;
				}
				else s1 = period / _TwoPi * (float)Math.Asin( 1 / overshootOrAmplitude );
				return ( overshootOrAmplitude * (float)Math.Pow( 2, -10 * time ) * (float)Math.Sin( ( time * duration - s1 ) * _TwoPi / period ) + 1 );
			case Ease.InOutElastic:
				float s;
				if( time == 0 ) return 0;
				if( ( time /= duration * 0.5f ) == 2 ) return 1;
				if( period == 0 ) period = duration * ( 0.3f * 1.5f );
				if( overshootOrAmplitude < 1 )
				{
					overshootOrAmplitude = 1;
					s = period / 4;
				}
				else s = period / _TwoPi * (float)Math.Asin( 1 / overshootOrAmplitude );
				if( time < 1 ) return -0.5f * ( overshootOrAmplitude * (float)Math.Pow( 2, 10 * ( time -= 1 ) ) * (float)Math.Sin( ( time * duration - s ) * _TwoPi / period ) );
				return overshootOrAmplitude * (float)Math.Pow( 2, -10 * ( time -= 1 ) ) * (float)Math.Sin( ( time * duration - s ) * _TwoPi / period ) * 0.5f + 1;
			case Ease.InBack:
				return ( time /= duration ) * time * ( ( overshootOrAmplitude + 1 ) * time - overshootOrAmplitude );
			case Ease.OutBack:
				return ( ( time = time / duration - 1 ) * time * ( ( overshootOrAmplitude + 1 ) * time + overshootOrAmplitude ) + 1 );
			case Ease.InOutBack:
				if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * ( time * time * ( ( ( overshootOrAmplitude *= ( 1.525f ) ) + 1 ) * time - overshootOrAmplitude ) );
				return 0.5f * ( ( time -= 2 ) * time * ( ( ( overshootOrAmplitude *= ( 1.525f ) ) + 1 ) * time + overshootOrAmplitude ) + 2 );
			case Ease.InBounce:
				return Bounce.EaseIn( time, duration, overshootOrAmplitude, period );
			case Ease.OutBounce:
				return Bounce.EaseOut( time, duration, overshootOrAmplitude, period );
			case Ease.InOutBounce:
				return Bounce.EaseInOut( time, duration, overshootOrAmplitude, period );
			case Ease.INTERNAL_Custom:
				return customEase( time, duration, overshootOrAmplitude, period );
			case Ease.INTERNAL_Zero:
				// 0 duration tween
				return 1;

			// Extra custom eases ////////////////////////////////////////////////////
			case Ease.Flash:
				return Flash.Ease( time, duration, overshootOrAmplitude, period );
			case Ease.InFlash:
				return Flash.EaseIn( time, duration, overshootOrAmplitude, period );
			case Ease.OutFlash:
				return Flash.EaseOut( time, duration, overshootOrAmplitude, period );
			case Ease.InOutFlash:
				return Flash.EaseInOut( time, duration, overshootOrAmplitude, period );

			// Default
			default:
				// OutQuad
				return -( time /= duration ) * ( time - 2 );
		}
	}

	public static EaseFunction ToEaseFunction( Ease ease )
	{
		switch( ease )
		{
			case Ease.Linear:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					time / duration;
			case Ease.InSine:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					-(float)Math.Cos( time / duration * _PiOver2 ) + 1;
			case Ease.OutSine:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					(float)Math.Sin( time / duration * _PiOver2 );
			case Ease.InOutSine:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					-0.5f * ( (float)Math.Cos( Mathf.PI * time / duration ) - 1 );
			case Ease.InQuad:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( time /= duration ) * time;
			case Ease.OutQuad:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					-( time /= duration ) * ( time - 2 );
			case Ease.InOutQuad:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time;
					return -0.5f * ( ( --time ) * ( time - 2 ) - 1 );
				};
			case Ease.InCubic:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( time /= duration ) * time * time;
			case Ease.OutCubic:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( ( time = time / duration - 1 ) * time * time + 1 );
			case Ease.InOutCubic:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time * time;
					return 0.5f * ( ( time -= 2 ) * time * time + 2 );
				};
			case Ease.InQuart:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( time /= duration ) * time * time * time;
			case Ease.OutQuart:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					-( ( time = time / duration - 1 ) * time * time * time - 1 );
			case Ease.InOutQuart:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time * time * time;
					return -0.5f * ( ( time -= 2 ) * time * time * time - 2 );
				};
			case Ease.InQuint:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( time /= duration ) * time * time * time * time;
			case Ease.OutQuint:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( ( time = time / duration - 1 ) * time * time * time * time + 1 );
			case Ease.InOutQuint:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * time * time * time * time * time;
					return 0.5f * ( ( time -= 2 ) * time * time * time * time + 2 );
				};
			case Ease.InExpo:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( time == 0 ) ? 0 : (float)Math.Pow( 2, 10 * ( time / duration - 1 ) );
			case Ease.OutExpo:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( time == duration ) return 1;
					return ( -(float)Math.Pow( 2, -10 * time / duration ) + 1 );
				};
			case Ease.InOutExpo:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( time == 0 ) return 0;
					if( time == duration ) return 1;
					if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * (float)Math.Pow( 2, 10 * ( time - 1 ) );
					return 0.5f * ( -(float)Math.Pow( 2, -10 * --time ) + 2 );
				};
			case Ease.InCirc:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					-( (float)Math.Sqrt( 1 - ( time /= duration ) * time ) - 1 );
			case Ease.OutCirc:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					(float)Math.Sqrt( 1 - ( time = time / duration - 1 ) * time );
			case Ease.InOutCirc:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( ( time /= duration * 0.5f ) < 1 ) return -0.5f * ( (float)Math.Sqrt( 1 - time * time ) - 1 );
					return 0.5f * ( (float)Math.Sqrt( 1 - ( time -= 2 ) * time ) + 1 );
				};
			case Ease.InElastic:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					float s0;
					if( time == 0 ) return 0;
					if( ( time /= duration ) == 1 ) return 1;
					if( period == 0 ) period = duration * 0.3f;
					if( overshootOrAmplitude < 1 )
					{
						overshootOrAmplitude = 1;
						s0 = period / 4;
					}
					else s0 = period / _TwoPi * (float)Math.Asin( 1 / overshootOrAmplitude );
					return -( overshootOrAmplitude * (float)Math.Pow( 2, 10 * ( time -= 1 ) ) * (float)Math.Sin( ( time * duration - s0 ) * _TwoPi / period ) );
				};
			case Ease.OutElastic:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					float s1;
					if( time == 0 ) return 0;
					if( ( time /= duration ) == 1 ) return 1;
					if( period == 0 ) period = duration * 0.3f;
					if( overshootOrAmplitude < 1 )
					{
						overshootOrAmplitude = 1;
						s1 = period / 4;
					}
					else s1 = period / _TwoPi * (float)Math.Asin( 1 / overshootOrAmplitude );
					return ( overshootOrAmplitude * (float)Math.Pow( 2, -10 * time ) * (float)Math.Sin( ( time * duration - s1 ) * _TwoPi / period ) + 1 );
				};
			case Ease.InOutElastic:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					float s;
					if( time == 0 ) return 0;
					if( ( time /= duration * 0.5f ) == 2 ) return 1;
					if( period == 0 ) period = duration * ( 0.3f * 1.5f );
					if( overshootOrAmplitude < 1 )
					{
						overshootOrAmplitude = 1;
						s = period / 4;
					}
					else s = period / _TwoPi * (float)Math.Asin( 1 / overshootOrAmplitude );
					if( time < 1 ) return -0.5f * ( overshootOrAmplitude * (float)Math.Pow( 2, 10 * ( time -= 1 ) ) * (float)Math.Sin( ( time * duration - s ) * _TwoPi / period ) );
					return overshootOrAmplitude * (float)Math.Pow( 2, -10 * ( time -= 1 ) ) * (float)Math.Sin( ( time * duration - s ) * _TwoPi / period ) * 0.5f + 1;
				};
			case Ease.InBack:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( time /= duration ) * time * ( ( overshootOrAmplitude + 1 ) * time - overshootOrAmplitude );
			case Ease.OutBack:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					( ( time = time / duration - 1 ) * time * ( ( overshootOrAmplitude + 1 ) * time + overshootOrAmplitude ) + 1 );
			case Ease.InOutBack:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
				{
					if( ( time /= duration * 0.5f ) < 1 ) return 0.5f * ( time * time * ( ( ( overshootOrAmplitude *= ( 1.525f ) ) + 1 ) * time - overshootOrAmplitude ) );
					return 0.5f * ( ( time -= 2 ) * time * ( ( ( overshootOrAmplitude *= ( 1.525f ) ) + 1 ) * time + overshootOrAmplitude ) + 2 );
				};
			case Ease.InBounce:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					Bounce.EaseIn( time, duration, overshootOrAmplitude, period );
			case Ease.OutBounce:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					Bounce.EaseOut( time, duration, overshootOrAmplitude, period );
			case Ease.InOutBounce:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					Bounce.EaseInOut( time, duration, overshootOrAmplitude, period );

			// Extra custom eases ////////////////////////////////////////////////////
			case Ease.Flash:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					Flash.Ease( time, duration, overshootOrAmplitude, period );
			case Ease.InFlash:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					Flash.EaseIn( time, duration, overshootOrAmplitude, period );
			case Ease.OutFlash:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					Flash.EaseOut( time, duration, overshootOrAmplitude, period );
			case Ease.InOutFlash:
				return ( float time, float duration, float overshootOrAmplitude, float period ) =>
					Flash.EaseInOut( time, duration, overshootOrAmplitude, period );

			// Default
			default:
				// OutQuad
				return ( float time, float duration, float overshootOrAmplitude, float period ) => -( time /= duration ) * ( time - 2 );
		}
	}

	internal static bool IsFlashEase( Ease ease )
	{
		switch( ease )
		{
			case Ease.Flash:
			case Ease.InFlash:
			case Ease.OutFlash:
			case Ease.InOutFlash:
				return true;
		}
		return false;
	}
}

/// <summary>
/// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
/// </summary>
public static class Bounce
{
	/// <summary>
	/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
	/// </summary>
	/// <param name="time">
	/// Current time (in frames or seconds).
	/// </param>
	/// <param name="duration">
	/// Expected easing duration (in frames or seconds).
	/// </param>
	/// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
	/// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
	/// <returns>
	/// The eased value.
	/// </returns>
	public static float EaseIn( float time, float duration, float unusedOvershootOrAmplitude, float unusedPeriod )
	{
		return 1 - EaseOut( duration - time, duration, -1, -1 );
	}

	/// <summary>
	/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
	/// </summary>
	/// <param name="time">
	/// Current time (in frames or seconds).
	/// </param>
	/// <param name="duration">
	/// Expected easing duration (in frames or seconds).
	/// </param>
	/// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
	/// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
	/// <returns>
	/// The eased value.
	/// </returns>
	public static float EaseOut( float time, float duration, float unusedOvershootOrAmplitude, float unusedPeriod )
	{
		if( ( time /= duration ) < ( 1 / 2.75f ) )
		{
			return ( 7.5625f * time * time );
		}
		if( time < ( 2 / 2.75f ) )
		{
			return ( 7.5625f * ( time -= ( 1.5f / 2.75f ) ) * time + 0.75f );
		}
		if( time < ( 2.5f / 2.75f ) )
		{
			return ( 7.5625f * ( time -= ( 2.25f / 2.75f ) ) * time + 0.9375f );
		}
		return ( 7.5625f * ( time -= ( 2.625f / 2.75f ) ) * time + 0.984375f );
	}

	/// <summary>
	/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="time">
	/// Current time (in frames or seconds).
	/// </param>
	/// <param name="duration">
	/// Expected easing duration (in frames or seconds).
	/// </param>
	/// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
	/// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
	/// <returns>
	/// The eased value.
	/// </returns>
	public static float EaseInOut( float time, float duration, float unusedOvershootOrAmplitude, float unusedPeriod )
	{
		if( time < duration * 0.5f )
		{
			return EaseIn( time * 2, duration, -1, -1 ) * 0.5f;
		}
		return EaseOut( time * 2 - duration, duration, -1, -1 ) * 0.5f + 0.5f;
	}
}

public static class Flash
{
	public static float Ease( float time, float duration, float overshootOrAmplitude, float period )
	{
		int stepIndex = Mathf.CeilToInt( ( time / duration ) * overshootOrAmplitude ); // 1 to overshootOrAmplitude
		float stepDuration = duration / overshootOrAmplitude;
		time -= stepDuration * ( stepIndex - 1 );
		float dir = ( stepIndex % 2 != 0 ) ? 1 : -1;
		if( dir < 0 ) time -= stepDuration;
		float res = ( time * dir ) / stepDuration;
		return WeightedEase( overshootOrAmplitude, period, stepIndex, stepDuration, dir, res );
	}

	public static float EaseIn( float time, float duration, float overshootOrAmplitude, float period )
	{
		int stepIndex = Mathf.CeilToInt( ( time / duration ) * overshootOrAmplitude ); // 1 to overshootOrAmplitude
		float stepDuration = duration / overshootOrAmplitude;
		time -= stepDuration * ( stepIndex - 1 );
		float dir = ( stepIndex % 2 != 0 ) ? 1 : -1;
		if( dir < 0 ) time -= stepDuration;
		time = time * dir;
		float res = ( time /= stepDuration ) * time;
		return WeightedEase( overshootOrAmplitude, period, stepIndex, stepDuration, dir, res );
	}

	public static float EaseOut( float time, float duration, float overshootOrAmplitude, float period )
	{
		int stepIndex = Mathf.CeilToInt( ( time / duration ) * overshootOrAmplitude ); // 1 to overshootOrAmplitude
		float stepDuration = duration / overshootOrAmplitude;
		time -= stepDuration * ( stepIndex - 1 );
		float dir = ( stepIndex % 2 != 0 ) ? 1 : -1;
		if( dir < 0 ) time -= stepDuration;
		time = time * dir;
		float res = -( time /= stepDuration ) * ( time - 2 );
		return WeightedEase( overshootOrAmplitude, period, stepIndex, stepDuration, dir, res );
	}

	public static float EaseInOut( float time, float duration, float overshootOrAmplitude, float period )
	{
		int stepIndex = Mathf.CeilToInt( ( time / duration ) * overshootOrAmplitude ); // 1 to overshootOrAmplitude
		float stepDuration = duration / overshootOrAmplitude;
		time -= stepDuration * ( stepIndex - 1 );
		float dir = ( stepIndex % 2 != 0 ) ? 1 : -1;
		if( dir < 0 ) time -= stepDuration;
		time = time * dir;
		float res = ( time /= stepDuration * 0.5f ) < 1
			? 0.5f * time * time
			: -0.5f * ( ( --time ) * ( time - 2 ) - 1 );
		return WeightedEase( overshootOrAmplitude, period, stepIndex, stepDuration, dir, res );
	}

	static float WeightedEase( float overshootOrAmplitude, float period, int stepIndex, float stepDuration, float dir, float res )
	{
		float easedRes = 0;
		float finalDecimals = 0;
		// Use previous stepIndex in case of odd ones, so that back ease is not clamped
		if( dir > 0 && (int)overshootOrAmplitude % 2 == 0 ) stepIndex++;
		else if( dir < 0 && (int)overshootOrAmplitude % 2 != 0 ) stepIndex++;

		if( period > 0 )
		{
			float finalTruncated = (float)Math.Truncate( overshootOrAmplitude );
			finalDecimals = overshootOrAmplitude - finalTruncated;
			if( finalTruncated % 2 > 0 ) finalDecimals = 1 - finalDecimals;
			finalDecimals = ( finalDecimals * stepIndex ) / overshootOrAmplitude;
			easedRes = ( res * ( overshootOrAmplitude - stepIndex ) ) / overshootOrAmplitude;
		}
		else if( period < 0 )
		{
			period = -period;
			easedRes = ( res * stepIndex ) / overshootOrAmplitude;
		}
		float diff = easedRes - res;
		res += ( diff * period ) + finalDecimals;
		if( res > 1 ) res = 1;
		return res;
	}
}