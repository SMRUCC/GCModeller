
#include "Log Separate.h"
#include <stdlib.h>
#include <math.h>
#include <windows.h>
#include <malloc.h>

// Original function name: LOGSEG
void SegmentSignal(double LOG[], int NSAMPS, double F, int ORDER, int ORDER1, int RMODE, int NITER, double Q[], int &NQ, double FLTLOG[], double SEGLOG[], double R[], double &C, double &D, int &NACT, int &IER)
{
	// Function:
	// Maximum Likelihood Segmentation
	//
	// Input parameters:
	// LOG: Input discretized well log.
	//
	// NSAMPS: Number of samples in array log.
	//
	// F: Segmentation threshold index.
	//
	// ORDER: Length of the moving average window used for smoothing the input well
	//			log to arrive at an initial estimate of jump sequence variance, C.
	//
	// ORDER1: Length of the moving average window used for smoothing the noise variances.
	//
	// RMODE: Noise variance estimate option.
	//		RMODE = 0 ==> Point estimate of noise variance.
	//		RMODE = 1 ==> Noise estimates smoothed within segments.
	//
	// NITER: Upper bound on the number of SingleMostLikelihoodReplacement iterations.
	//
	// Output parameters:
	// Q: Binary event sequence.
	//
	// NQ: Number of binary events found (number of "1"s in Q).
	//
	// FLTLOG: Filtered estimate of the signal.  Also used as intermediate work array.
	//
	// SEGLOG: FLTLOG averaged over each segment.
	//
	// R: Estimated noise variance.
	//
	// C: Estimated variance of the jump sequence.
	//
	// D: Estimated segment density.
	//
	// NACT: Actual SingleMostLikelihoodReplacement iteration count.
	//
	// IER: Error flag.
	//		IER = 0 ==> No error.
	//		IER < 0 ==> Invalid event density estimated	after threshold.  Reduce/increase F and rerun.
	//		IER > 0 ==> Logarithm argument became zero at sample number IER during the calculation of
	//			likelihood ratios in SingleMostLikelihoodReplacement.  There may be more samples of this type which may give rise
	//			to this problem.  Edit/rescale data values and rerun.
	
	// Intermediate work arrays.
	double *WORK1 = (double*)malloc(NSAMPS * sizeof(double));
	double *WORK2 = (double*)malloc(NSAMPS * sizeof(double));

	int HORDER	= ORDER / 2;
	int NORDER	= ORDER1 / 2;

	// Ensure we did not round to 0.
	if (HORDER < 1)
	{
		HORDER = 1;
	}

	if (NORDER < 1)
	{
		NORDER = 1;
	}

	int VMODE	= 0;
	bool CONV	= false;
	int ITER	= 0;

	double FLPONE	= 1.0;
	double MEAN		= 0.0;
	double LBOUND	= 0.1E-04;
	double UBOUND	= 0.98;

	// Smooth input log using an HORDER point moving average (HORDER is number of points).
	MovingAverage(LOG, NSAMPS, HORDER, SEGLOG);

	// Initial estimate of c the variance of the jump sequence.
	EstimateVarianceOfJumpSequence(SEGLOG, NSAMPS, WORK1, C);

	// Estimate R, the variance of the noise.
	for (int i = 0; i < NSAMPS; i++)
	{
		SEGLOG[i] = (LOG[i]-SEGLOG[i]) * (LOG[i]-SEGLOG[i]);
	}

	// Smooth the variance of the noise using an NORDER point moving average.
	MovingAverage(SEGLOG, NSAMPS, NORDER, R);

	// Bootstrap event sequence.
	for (int i = 0; i < 2; i++)
	{
		// Initialize event sequence Q.
		for (int j = 0; j < NSAMPS; j++)
		{
			Q[j] = FLPONE;
		}
		
		// Invoke fixed interval optimal smoother to estimate the jump sequence, residual state, and its variance.
		KalmanFilter(LOG, Q, C, R, NSAMPS, WORK2, WORK1, SEGLOG, FLTLOG);
		
		FixedIntervalOptimalSmoother(Q, NSAMPS, C, SEGLOG, WORK1, WORK2);

		// Re-estimate the variance of the jump sequence C.
		CalculateMeanAndVariance(SEGLOG, NSAMPS, VMODE, MEAN, C);
		
		// Invoke threshold detector and update the event sequence.
		Threshold(SEGLOG, C, F, NSAMPS, Q, D);

		// Deglitch the event sequence Q.
		Deglitch(NSAMPS, Q);
		
		// Update d, the event density.
		EstimateEventDensity(Q, NSAMPS, D, NQ);
		C = C / D;
		
		// Deglitch and re-estimate R, the variance of noise.
		DeglitchAndEstimateNoiseVariance(LOG, Q, NSAMPS, RMODE, SEGLOG, R);
	}

	// Trap event density if invalid.
	if (D <= LBOUND || D >= UBOUND)
	{
		IER = -1;
	}
	else
	{
		// Start Single Most Likelihood Replacement iterations.
		do
		{
			// Increment iteration counter.
			ITER++;

			// Invoke fixed interval optimal smoother to estimate jump sequence, residual state, and its variance.
			KalmanFilter(LOG, Q, C, R, NSAMPS, WORK2, WORK1, SEGLOG, FLTLOG);

			FixedIntervalOptimalSmoother(Q, NSAMPS, C, SEGLOG, WORK1, WORK2);

			// Invoke Single Most Likelihood Replacement detector.
			SingleMostLikelihoodReplacement(WORK2, WORK1, C, NSAMPS, D, Q, CONV, IER);

			// Update event density d.
			EstimateEventDensity(Q, NSAMPS, D, NQ);

		}
		// Check for SingleMostLikelihoodReplacement termination.
		while (ITER < NITER && !CONV && IER == 0);

		NACT = ITER;

		// Re-run KalmanFilter filter to estimate the filtered log if SingleMostLikelihoodReplacement did not converge.
		if (CONV != 1 && IER == 0)
		{
			KalmanFilter(LOG, Q, C, R, NSAMPS, WORK2, WORK1, SEGLOG, FLTLOG);
		}
		// Average filtered log over segments.
		AverageArraySegments(FLTLOG, Q, NSAMPS, SEGLOG);
	}

	// Free memory.
	free(WORK1);
	free(WORK2);
}

// Original function name: SMLR
void SingleMostLikelihoodReplacement(double G[], double S[], double C, int NSAMPS, double D, double Q[], bool &CONV, int &IER)
{
	// Function:
	// Single most likelihood replacement detector for updating an event sequence Q(k).
	//
	// Input parameters:
	// G: Residual state.
	//
	// S: Variance of the residual state.
	//
	// C: Variance of the jump sequence.
	//
	// NSAMPS: Length of the binary event sequence.
	//
	// D: Segment density.
	//
	// Input/output parameters:
	// Q: Binary event sequence.  This sequence is updated by SingleMostLikelihoodReplacement.
	//
	// Output parameters:
	// CONV: Convergence flag.
	//		CONV = 0 => No convergence.
	//		CONV = 1 => SingleMostLikelihoodReplacement convergence.
	//
	// IER: Return error code.
	//		IER = 0 => No error.
	//		IER = 1 => logarithm argument became zero at sample number IER.

	// Input initialization.
	IER		= 0;
	CONV	= 0;

	// Local variable initialization.
	double LNHOLD	= log(D / (1.0-D));
	int INDKNT		= 0;
	int INDSTR		= 0;
	double GLOBAL	= 0;

	// Calculate log-likelihood ratios.
	for (int i = 0; i < NSAMPS; i++)
	{
		double DHOLD	= 1.0 - 2.0 * Q[i];
		double LOGARG	= 1.0 + C * DHOLD * S[i];

		if (LOGARG <= 0)
		{
			IER = i;
			return;
		}
		
		double HOLD3 = -0.5 * log(LOGARG);
		double HOLD1 = C * G[i] * G[i] * DHOLD / (2.0*LOGARG);
		double STORE = HOLD1 + DHOLD * LNHOLD + HOLD3;
		if (STORE > 0)
		{
			INDKNT++;
			if (STORE > GLOBAL)
			{
				GLOBAL = STORE;
				INDSTR = i;
			}
		}
	}

	if (INDKNT < 1)
	{
		// In case of convergence, return.
		CONV = true;
	}
	else
	{
		// Reset event.
		if (Q[INDSTR] > 0.01)
		{
			Q[INDSTR] = 0.0;
		}
		else
		{
			Q[INDSTR] = 1.0;
		}
	}
}

// Original function name: THOLD
void Threshold(double JMPSEQ[], double C, double F, int NSAMPS, double Q[], double &D)
{
	// Function:
	// Threshold procedure to estimate an initial event sequence.
	//
	// Input parameters:
	// JMPSEQ: Jump sequence estimate obtained with the event sequence Q set to unity for all values.
	//
	// C: Variance of the jump sequence.
	//
	// F: Segmentation threshold index.
	//
	// NSAMPS: Length of the array JMPSEQ.
	//
	// Output parameters:
	// Q: Estimated binary event sequence.
	//
	// D: Estimated segment density.

	D = 0.0;

	for (int i = 0; i < NSAMPS; i++)
	{
 		if (F*C <= JMPSEQ[i]*JMPSEQ[i])
		{
			Q[i]	= 1.0;
			D		= D + 1.0;
		}
		else
		{
			Q[i] = 0.0;
		}
	}

	D	= D / (double)NSAMPS;
}

// Original function name: MOVAVG
void MovingAverage(double ARR1[], int NSAMPS, int ORDER, double ARR2[])
{
	// Function:
	// Centered moving average.
	//
	// Input parameters:
	// ARR1: Input data array to be averaged.
	//
	// NSAMPS: Length of the input data array (ARR1).
	//
	// ORDER: One sided length of the moving average window.
	//
	// Output parameters:
	// ARR2: Averaged data array.

	if (ORDER >= NSAMPS)
	{
		// Auto correct.
		ORDER = NSAMPS-2;
	}

	ARR2[0]			= ARR1[0];
	ARR2[NSAMPS-1]	= ARR1[NSAMPS-1];

	// Average build & drop (beginning and end of average array).
	double SUM = ARR1[0];
	double SUM1 = ARR1[NSAMPS-1];
	
	int jfront = -1;
	int jback = NSAMPS;

	for (int i = 1; i < ORDER+1; i++)
	{
		// Building by 2 because this is a "central" moving average.
		jfront = jfront + 2;
		jback = jback - 2;
		SUM = SUM + ARR1[jfront] + ARR1[jfront+1];
		SUM1 = SUM1 + ARR1[jback] + ARR1[jback-1];
		ARR2[i] = SUM / (jfront + 2);
		ARR2[NSAMPS-1-i] = SUM1 / (jfront + 2);
	}

	// Average stable.
	jfront	= jfront + 2;
	jback	= 0;

	for (int i = ORDER+1; i < NSAMPS-ORDER; i++)
	{
		SUM = SUM + ARR1[jfront] - ARR1[jback];
		ARR2[i] = SUM / (ORDER * 2 + 1);
		jfront++;
		jback++;
	}
}

// Original function name: KALMAN
void KalmanFilter(double LOG[], double Q[], double C, double R[], int NSAMPS, double GAIN[], double ZTLT[], double ETA[], double STATE[])
{
	// Function:
	// One dimensional KalmanFilter filter.
	//
	// Input parameters:
	// LOG: Input discretized well log.
	//
	// Q: Binary event sequence.
	//
	// C: Variance of the jump sequence.
	//
	// NSAMPS: Length of the input well log.
	//
	// Output parameters:
	// GAIN: KalmanFilter gain array.
	//
	// ZTLT: Innovation sequence.
	//
	// ETA: Variance of the innovation sequence.
	//
	// STATE: Estimated state values (filtered log).

	// Init state & its variance.
	double VAR = 0.0;

	if (Q[0] > 0.1)
	{
		VAR = C;
	}

	ZTLT[0]		= 0.0;
	ETA[0]		= VAR + R[0];
	GAIN[0]		= VAR / ETA[0];
	STATE[0]	= LOG[0] + GAIN[0] * ZTLT[0];
	VAR			= (1.0 - GAIN[0]) * VAR;

	// Estimate state & its variance (one dim KalmanFilter filter).
	for (int i = 1; i < NSAMPS; i++)
	{
		if (Q[i] > 0.1)
		{
			VAR = VAR + C;
		}

		ZTLT[i]		= LOG[i] - STATE[i-1];
		ETA[i]		= VAR + R[i];
		GAIN[i]		= VAR / ETA[i];
		STATE[i]	= STATE[i-1] + GAIN[i] * ZTLT[i];
		VAR			= (1.0 - GAIN[i]) * VAR;
	}
}

// Original function name: FIOS
void FixedIntervalOptimalSmoother(double Q[], int NSAMPS, double C, double ARR1[], double ARR2[], double ARR3[])
{
	// Function:
	// Fixed interval optimal smoother to estimate the jump sequence, residual state and its variance.
	//
	// Input parameters:
	// Q: Binary event sequence.
	//
	// NSAMPS: Length of the binary event sequence Q.
	//
	// C: Variance of the jump sequence.
	//
	// Input/output parameters:
	// ARR1: Variance of the innovation sequence.  On return it contains the product Q*R.
	//
	// ARR2: Innovation sequence.  On return it contains S (variance of residual state).
	//
	// ARR3: KalmanFilter gain array.  On return it contains G (residual state).

	int lastindex = NSAMPS - 1;

	ARR3[lastindex] = ARR2[lastindex] / ARR1[lastindex];
	ARR2[lastindex] = 1.0 / ARR1[lastindex];
	ARR1[lastindex] = C * Q[lastindex] * ARR3[lastindex];

	for (int i = lastindex-1; i > -1; i--)
	{
		double TEMP = 1.0 - ARR3[i];
		ARR3[i] = TEMP * ARR3[i+1] + ARR2[i] / ARR1[i];
		ARR2[i] = TEMP * TEMP * ARR2[i+1] + 1.0 / ARR1[i];
		ARR1[i] = C * Q[i] * ARR3[i];
	}
}

// Original function name: DATAVG
void AverageArraySegments(double ARR[], double Q[], int NSAMPS, double AVGOUT[])
{
	// Function:
	// Data array to be averaged within segments implied by an event sequence Q.
	//
	// Input parameters:
	// ARR: Input data array to be averaged.
	//
	// Q: Binary event sequence.
	//
	// NSAMPS: Length of the input data array (ARR).
	//
	// Output parameters:
	// AVGOUT: Output data averaged within each segment implied by the event sequence Q.

	bool endofarray	= false;
	int SEGSTR		= 0;

	while (!endofarray)
	{
		int SEGEND;
		int SEGLEN;
		double SEGAVG;

		GetSegment(ARR, Q, NSAMPS, SEGSTR, SEGEND, SEGLEN, SEGAVG, endofarray);

		for (int i = SEGSTR; i <= SEGEND; i++)
		{
			AVGOUT[i] = SEGAVG;
		}

		SEGSTR	= SEGEND + 1;
	}
}

// Original function name: UPDNSE
void DeglitchAndEstimateNoiseVariance(double LOG[], double Q[], int NSAMPS, int RMODE, double WORK1[], double R[])
{
	// Function:
	// Deglitch and re-estimate noise variance.
	//
	// Input parameters:
	// LOG: Input discretized well log to be used for updating noise variance R.
	//
	// Q: Binary event sequence.
	//
	// NSAMPS: Length of the input well log.
	//
	// RMODE: Noise variance estimate option.
	//		RMODE = 0 ==> Point estimate of noise variance.
	//		RMODE = 1 ==> Noise estimates smoothed within segments.
	//
	// Output parameters:
	// R: Estimated noise variance.

	// Average the log within each segment implied by Q.
	AverageArraySegments(LOG, Q, NSAMPS, WORK1);

	// Calculate the noise variance R.
	for (int i = 0; i < NSAMPS; i++)
	{
		WORK1[i] = (LOG[i] - WORK1[i]) * (LOG[i] - WORK1[i]);
		
		if (WORK1[i] <= 0.0)
		{
			WORK1[i] = 1.0;
		}
	}

	// Point estimate.
	if (RMODE == 0)
	{
		for (int i = 0; i < NSAMPS; i++)
		{
			R[i] = WORK1[i];
		}
	}
	else
	{
		// Smooth the noise variance R within each segment.
		int SEGSTR = 0;
		bool endofarray = false;

		while (!endofarray)
		{
			// Outputs from call to "GetSegment".
			int SEGEND;
			int SEGLEN;
			double SEGAVG;

			GetSegment(LOG, Q, NSAMPS, SEGSTR, SEGEND, SEGLEN, SEGAVG, endofarray);

			int IOR = SEGLEN/3;
			if (IOR > 0)
			{
				MovingAverage(&(WORK1[SEGSTR]), SEGLEN, IOR, &(R[SEGSTR]));
			}
			else
			{
				for (int i = SEGSTR; i <= SEGEND; i++)
				{
					R[i] = WORK1[i];
				}
			}
		
			SEGSTR = SEGEND + 1;
		}
	}
}

// Original function name: GETSEG
void GetSegment(double LOG[], double Q[], int NSAMPS, int SEGSTR, int &SEGEND, int &SEGLEN, double &SEGAVG, bool &endofarray)
{
	// Function:
	// Identifies a segment within a given event sequence.
	// A segment is defined by the sequence 1,0,0,...,0,1.
	//
	// Input parameters:
	// LOG: Data array for which the averages are to be computed.
	//
	// Q: Binary event sequence which identifies the segments in the input array log.
	//
	// NSAMPS: Length of the input array log.
	//
	// SEGSTR: Sample number where the segment boundary starts.
	//
	// Output parameters:
	// SEGEND: Sample number where the segment boundary ends.
	//
	// SEGLEN: Length of the segment.
	//
	// SEGAVG: Average value of the input data array log over the segment.
	//
	// endofarray: End of data indicator.
	//		true: End of array reached.
	//		false: Not at end of the array.

	endofarray	= false;
	SEGEND		= SEGSTR;
	SEGAVG		= 0;

	do
	{
		SEGAVG = SEGAVG + LOG[SEGEND];
		SEGEND = SEGEND + 1;
	}
	while (Q[SEGEND] < 0.5 && SEGEND < NSAMPS);

	if (SEGEND == NSAMPS)
	{
		// We hit the end of the array without finding an event indicator (a "1" in the "Q" array).
		endofarray = true;
	}

	// In the case were we found a "1" in the "Q" array the end of the segment is the previous
	// index before the event ("1").  If we hit the end of the array first, SEGEND == NSAMPS which
	// is past the end of the array so it needs to be de-incremented.
	SEGEND--;

	SEGLEN = SEGEND - SEGSTR + 1;
	SEGAVG = SEGAVG / SEGLEN;
}

// Original function name: DEGLCH
void Deglitch(int NSAMPS, double Q[])
{
	// Function:
	// Deglitches all the spiky zones in the event sequence Q.
	//
	// Input parameters:
	// NSAMPS: Length of the array Q.
	//
	// Input/output parameters:
	// Q: Binary event sequence to be deglitched.  On return this contains the updated event
	//		sequence.  It is of length "NSAMPS."

	int SEGSTR		= 0;
	bool endofarray = false;

	do
	{
		int SEGEND;
		GetSpikeyZone(Q, NSAMPS, SEGSTR, SEGEND, endofarray);

		if (SEGSTR == SEGEND)
		{
			SEGSTR++;
		}
		else
		{
			if (SEGEND == SEGSTR+1)
			{
				Q[SEGSTR] = 0;
			}
			else
			{
				for (int i = SEGSTR+1; i < SEGEND; i++)
				{
					Q[i] = 0;
				}
			}
			SEGSTR = SEGEND + 1;
		}
	}
	while (!endofarray);
}

// Original function name: GETSPK
void GetSpikeyZone(double Q[], int NSAMPS, int SEGSTR, int &SEGEND, bool &endofarray)
{
	// Function:
	// Identifies a zone of spikes.  A spiky zone is defined by 0,1,1,...,1,0.
	//
	// Input parameters:
	// Q: Binary event sequence in which the zone is to be identified.
	//
	// NSAMPS: Length of the array Q.
	//
	// SEGSTR: Sample number where the spiky zone starts.
	//
	// Output parameters:
	// SEGEND: Sample number where the zone ends.
	//
	// endofarray: End of data indicator.
	//		true: End of array reached.
	//		false: Not at end of the array.

	SEGEND = SEGSTR;
	if (Q[SEGEND] < 0.5)
	{
		// Modified from original paper.  The "if" statement below was not part of the original algorithm.
		// It seems there can be a case where Q[i] < 0.5 for SEGEND = NSAMPS that causes us to miss flagging the end
		// of the array.  Do a check here to ensure we don't overrun the end of the array and over write memory we should
		// not over write.
		if (SEGEND >= NSAMPS)
		{
			endofarray	= true;
			SEGEND		= NSAMPS - 1;
		}

		return;
	}

	do
	{
		SEGEND++;
	}
	while ((Q[SEGEND] > 0.5) && (SEGEND < NSAMPS));

	if (SEGEND >= NSAMPS)
	{
		endofarray = true;
	}

	SEGEND--;
}

// Original function name: UPDDEN
void EstimateEventDensity(double Q[], int NSAMPS, double &D, int &NQ)
{
	// Function:
	// Estimates the event density for a given event sequence.
	//
	// Input parameters:
	// Q: Binary event sequence for which the event density is to be estimated.
	//
	// NSAMPS: Length of the array Q.
	//
	// Output parameters:
	// D: Estimated segment density.
	//
	// NQ: Number of binary events found (number of "1"s in Q).

	D = 0;
	for (int i = 0; i < NSAMPS; i++)
	{
		D = D + Q[i];
	}

	// Store the number of events found, the calculate the event density.
	NQ	= (int)D;
	D = D / (double)NSAMPS;
}

// Original function name: ESTSIG
void EstimateVarianceOfJumpSequence(double LOG[], int NSAMPS, double WORK1[], double &C)
{
	// Function:
	// Estimates initially C, the variance of the jump sequence as the variance of unit-step
	// finite difference of input smoothed log.
	//
	// Input parameters:
	// LOG: Smoothed well log.
	//
	// NSAMPS: Length of the array log.
	//
	// Output parameters:
	// C: Estimated variance of the jump sequence.

	// Estimate jump sequence as the unit-step finite difference of input log.
	WORK1[0] = 0;
	for (int i = 1; i < NSAMPS; i++)
	{
		WORK1[i] = LOG[i] - LOG[i-1];
	}

	// Calculate the variance of the differ log (consider non zero values).
	double MEAN;
	CalculateMeanAndVariance(WORK1, NSAMPS, 1, MEAN, C);
}

// Original function name: VARNCE
void CalculateMeanAndVariance(double ARRAY[], int NSAMPS, int MODE, double &MEAN, double &VAR)
{
	// Function:
	// Calculates mean and variance.
	//
	// Input parameters:
	// ARRAY: Input data array for which the mean and variance is to be calculated.
	//
	// NSAMPS: Length of the input array.
	//
	// MODE: Option to include zero values for calculating mean and variance.
	//		MODE == 0 = Consider all values.
	//		MODE == 1 = Consider only non zero values.
	//
	// Output parameters:
	// MEAN: Calculated mean of the given data.
	//
	// VAR: Calculated variance of the given data.

	int NZEKNT	= 0;
	MEAN		= 0;
	VAR			= 0;

	// Calculate sum and sum of squares.
	for (int i = 0; i < NSAMPS; i++)
	{

		if (ARRAY[i] != 0)
		{
			NZEKNT++;
			MEAN	= MEAN + ARRAY[i];
			VAR		= VAR + ARRAY[i]*ARRAY[i];
		}
	}

	// Calculate mean and variance.
	if (MODE == 0)
	{
		MEAN = MEAN / (double)NSAMPS;
		VAR = VAR / (double)NSAMPS - MEAN*MEAN;
	}
	else
	{
		MEAN = MEAN / (double)NZEKNT;
		VAR = VAR / (double)NZEKNT - MEAN*MEAN;
	}
}