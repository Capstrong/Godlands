using UnityEngine;
using System.Collections;

public class MathUtils
{
	/**
	 * @brief An arithmetically correct modulos operation.
	 *
	 * @details
	 *     The % operator is not actually a mod operator, it
	 *     only does remainder, so doing -3 % 5 returns -3,
	 *     where it should return 2. This function calculates
	 *     the correct modulos value.
	 *
	 * @note
	 *     You only need to use this function if you expect
	 *     x to be a negative number. If you can be sure that
	 *     it will always be non-negative then you are safe
	 *     using the % operator.
	 */
	public static int Mod( int x, int m )
	{
		return ( x % m + m ) % m;
	}
}
