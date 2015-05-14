#ifndef GODLANDS_SHADER_UTILS_INCLUDED
#define GODLANDS_SHADER_UTILS_INCLUDED

////////////////
// NOISE UTIL ///
////////////////

#ifndef __noise_hlsl_
#define __noise_hlsl_
 
// hash based 3d value noise
// function taken from [url]https://www.shadertoy.com/view/XslGRr[/url]
// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
 
// ported from GLSL to HLSL
 
float hash( float n )
{
    return frac(sin(n)*43758.5453);
}
 
float pnoise( float3 x )
{
    // The noise function returns a value in the range -1.0f -> 1.0f
 
    float3 p = floor(x);
    float3 f = frac(x);
 
    f       = f*f*(3.0-2.0*f);
    float n = p.x + p.y*57.0 + 113.0*p.z;
 
    return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
                   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
                   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
}

#endif

///////////////////
// WORLD BENDING //
///////////////////

fixed _Curvature;			// How fierce is the curve (default is 0.001)
fixed _MinCurveDistance;	// How far from the camera does the curve start

inline fixed4 CalculateOffset( fixed4 vv )
{
    // Reduce the y coordinate (i.e. lower the "height") of each vertex based
    // on the square of the distance from the camera in the z axis, multiplied
    // by the chosen curvature factor
    fixed dist = sqrt( vv.z * vv.z + vv.x * vv.x );
    dist = clamp(dist - _MinCurveDistance, 0, 100000);
	return fixed4( 0.0f, (dist * dist) * -_Curvature, 0.0f, 0.0f );
}

inline fixed4 CalculateWorldBendOffset( appdata_full v )
{
	// Transform the vertex coordinates from model space into world space
	fixed4 vv = mul( _Object2World, v.vertex );
	
	// Now adjust the coordinates to be relative to the camera position
    // Need to clamp somewhere here so there's a little offset before starting
    vv.xyz -= _WorldSpaceCameraPos.xyz;
    
   	vv = CalculateOffset( vv );

    // Now apply the offset back to the vertices in model space
    return mul(_World2Object, vv);
}

struct appdata_t 
{
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

inline fixed4 CalculateWorldBendOffset( fixed4 vv )
{
    return CalculateOffset( vv );
    //return mul(_World2Object, vv);
}

//////////////////
// WIND BENDING //
//////////////////

fixed4 _WindDirection; // What direction is the wind blowing (remember that w should be zero)
fixed _WindStrength;   // How far do plants bend
fixed _WindVariance;   // How fast do plants wax/wane

inline float4 CalculateWindBendOffset( appdata_full v )
{
	fixed4 moveVec = (sin( _Time.y * _WindVariance + pnoise(length( v.vertex ))) * 0.5 + 0.4);
	moveVec *= mul(_World2Object, normalize(_WindDirection)); // Transform wind direction from world to object space
	
	fixed moveAmount = v.color.r * _WindStrength;
	
	return moveVec * moveAmount;
}

#endif // GODLANDS_SHADER_UTILS_INCLUDED