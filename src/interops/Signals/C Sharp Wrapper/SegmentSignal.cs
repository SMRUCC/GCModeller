using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// DLL support.
using System.Runtime.InteropServices;

namespace Algorithms
{
	/// <summary>
	/// C# wrapper for the C library.
	/// </summary>
	public class SegmentSignal
	{
		#region DLL Imports

		[DllImport("SegmentSignal.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SegmentSignalWrapper(double[] LOG, int NSAMPS, double F, int ORDER, int ORDER1, int RMODE, int NITER, double[] Q, out int NQ, double[] FLTLOG, double[] SEGLOG, double[] R, out double C, out double D, out int NACT, out int IER);

		#endregion

		#region Segment Functions

		#region Input Signal Provided as an Array of Doubles

		/// <summary>
		/// Segment a signal using the Maximum Likelihood Estimation of Radhakrishnan, et al, 1991.  Attempts to identify regions of the signal that are
		/// considered "consistent."  Assumes a signal that has a state which changes only at segment boundaries.  The state change can be random and the
		/// noise on top of the signal is modeled as Gaussian, but not necessarily stationary.
		/// 
		/// Assumes a point estimate of the noise variance and an upper bound of 300 on the number of Single Most Likelihood Replacement iterations.
		/// </summary>
		/// <param name="signal">Input signal to be segmented.</param>
		/// <param name="threshold">Segmentation threshold.</param>
		/// <param name="jumpSequenceWindowSize">Length of the moving average window sized used for smoothing the input well log to arrive at an initial estimate of the jump sequence variance.</param>
		/// <param name="noiseVarianceWindowSize">Length of the moving average window used for smoothing the noise variances.</param>
		/// <param name="noiseVarianceEstimateMethod">
		/// Noise variance estimate option.
		//		rmode = 0 ==> Point estimate of noise variance.
		//		rmode = 1 ==> Noise estimates smoothed within segments.
		/// </param>
		/// <returns>A SegmentationResults instance which contains the algorithm output of binary events, segmented log, filtered log, et cetera.</returns>
		public static SegmentationResults Segment(double[] signal, double threshold, int jumpSequenceWindowSize, int noiseVarianceWindowSize)
		{
			return Segment(signal, threshold, jumpSequenceWindowSize, noiseVarianceWindowSize, NoiseVarianceEstimateMethod.Point, 300);
		}

		/// <summary>
		/// Segment a signal using the Maximum Likelihood Estimation of Radhakrishnan, et al, 1991.  Attempts to identify regions of the signal that are
		/// considered "consistent."  Assumes a signal that has a state which changes only at segment boundaries.  The state change can be random and the
		/// noise on top of the signal is modeled as Gaussian, but not necessarily stationary.
		/// 
		/// Assumes an upper bound of 300 on the number of Single Most Likelihood Replacement iterations.
		/// </summary>
		/// <param name="signal">Input signal to be segmented.</param>
		/// <param name="threshold">Segmentation threshold.</param>
		/// <param name="jumpSequenceWindowSize">Length of the moving average window sized used for smoothing the input well log to arrive at an initial estimate of the jump sequence variance.</param>
		/// <param name="noiseVarianceWindowSize">Length of the moving average window used for smoothing the noise variances.</param>
		/// <param name="noiseVarianceEstimateMethod">
		/// Noise variance estimate option.
		//		rmode = 0 ==> Point estimate of noise variance.
		//		rmode = 1 ==> Noise estimates smoothed within segments.
		/// </param>
		/// <returns>A SegmentationResults instance which contains the algorithm output of binary events, segmented log, filtered log, et cetera.</returns>
		public static SegmentationResults Segment(double[] signal, double threshold, int jumpSequenceWindowSize, int noiseVarianceWindowSize, NoiseVarianceEstimateMethod noiseVarianceEstimateMethod)
		{
			return Segment(signal, threshold, jumpSequenceWindowSize, noiseVarianceWindowSize, noiseVarianceEstimateMethod, 300);
		}

		/// <summary>
		/// Segment a signal using the Maximum Likelihood Estimation of Radhakrishnan, et al, 1991.  Attempts to identify regions of the signal that are
		/// considered "consistent."  Assumes a signal that has a state which changes only at segment boundaries.  The state change can be random and the
		/// noise on top of the signal is modeled as Gaussian, but not necessarily stationary.
		/// </summary>
		/// <param name="signal">Input signal to be segmented.</param>
		/// <param name="threshold">Segmentation threshold.</param>
		/// <param name="jumpSequenceWindowSize">Length of the moving average window sized used for smoothing the input well log to arrive at an initial estimate of the jump sequence variance.</param>
		/// <param name="noiseVarianceWindowSize">Length of the moving average window used for smoothing the noise variances.</param>
		/// <param name="noiseVarianceEstimateMethod">
		/// Noise variance estimate option.
		//		rmode = 0 ==> Point estimate of noise variance.
		//		rmode = 1 ==> Noise estimates smoothed within segments.
		/// </param>
		/// <param name="maxSMLRIterations">Upper bound on the number of Single Most Likelihood Replacement iterations.</param>
		/// <returns>A SegmentationResults instance which contains the algorithm output of binary events, segmented log, filtered log, et cetera.</returns>
		public static SegmentationResults Segment(double[] signal, double threshold, int jumpSequenceWindowSize, int noiseVarianceWindowSize, NoiseVarianceEstimateMethod noiseVarianceEstimateMethod, int maxSMLRIterations)
		{
			// Scalars which are need for output from the call to the algorithm.
			int		numberOfBinaryEvents	= 0;
			double	jumpSequenceVariance	= 0;
			double	segmentDensity			= 0;
			int		iterations				= 0;
			int		error					= 0;

			// We seem to need to create the array in the function immediately prior to calling the unmanaged code, otherwise the garbage collector
			// gets cute and moves things around, which the unmanaged code takes offense to.
			double[] signalToPass = new double[signal.Length];
			signal.CopyTo(signalToPass, 0);

			// Create the output arrays.  After being populated by the call to the algorithm they are grouped, stored, and returned in the SegmenationResults data structure.
			double[] Q		= new double[signalToPass.Length];
			double[] FLTLOG = new double[signalToPass.Length];
			double[] SEGLOG	= new double[signalToPass.Length];
			double[] R		= new double[signalToPass.Length];

			// Function call to the C DLL.
			SegmentSignalWrapper(signalToPass, signalToPass.Length, threshold, jumpSequenceWindowSize, noiseVarianceWindowSize, (int)noiseVarianceEstimateMethod, maxSMLRIterations, Q, out numberOfBinaryEvents, FLTLOG, SEGLOG, R, out jumpSequenceVariance, out segmentDensity, out iterations, out error);

			return new SegmentationResults(Q, numberOfBinaryEvents, FLTLOG, SEGLOG, R, jumpSequenceVariance, segmentDensity, iterations, error);
		}

		#endregion

		#region Input Signal Provided as an List of Doubles

		/// <summary>
		/// Segment a signal using the Maximum Likelihood Estimation of Radhakrishnan, et al, 1991.  Attempts to identify regions of the signal that are
		/// considered "consistent."  Assumes a signal that has a state which changes only at segment boundaries.  The state change can be random and the
		/// noise on top of the signal is modeled as Gaussian, but not necessarily stationary.
		/// 
		/// Assumes a point estimate of the noise variance and an upper bound of 300 on the number of Single Most Likelihood Replacement iterations.
		/// </summary>
		/// <param name="signal">Input signal to be segmented.</param>
		/// <param name="threshold">Segmentation threshold.</param>
		/// <param name="jumpSequenceWindowSize">Length of the moving average window sized used for smoothing the input well log to arrive at an initial estimate of the jump sequence variance.</param>
		/// <param name="noiseVarianceWindowSize">Length of the moving average window used for smoothing the noise variances.</param>
		/// <param name="noiseVarianceEstimateMethod">
		/// Noise variance estimate option.
		//		rmode = 0 ==> Point estimate of noise variance.
		//		rmode = 1 ==> Noise estimates smoothed within segments.
		/// </param>
		/// <returns>A SegmentationResults instance which contains the algorithm output of binary events, segmented log, filtered log, et cetera.</returns>
		public static SegmentationResults Segment(List<double> signal, double threshold, int jumpSequenceWindowSize, int noiseVarianceWindowSize)
		{
			return Segment(signal, threshold, jumpSequenceWindowSize, noiseVarianceWindowSize, NoiseVarianceEstimateMethod.Point, 300);
		}

		/// <summary>
		/// Segment a signal using the Maximum Likelihood Estimation of Radhakrishnan, et al, 1991.  Attempts to identify regions of the signal that are
		/// considered "consistent."  Assumes a signal that has a state which changes only at segment boundaries.  The state change can be random and the
		/// noise on top of the signal is modeled as Gaussian, but not necessarily stationary.
		/// 
		/// Assumes an upper bound of 300 on the number of Single Most Likelihood Replacement iterations.
		/// </summary>
		/// <param name="signal">Input signal to be segmented.</param>
		/// <param name="threshold">Segmentation threshold.</param>
		/// <param name="jumpSequenceWindowSize">Length of the moving average window sized used for smoothing the input well log to arrive at an initial estimate of the jump sequence variance.</param>
		/// <param name="noiseVarianceWindowSize">Length of the moving average window used for smoothing the noise variances.</param>
		/// <param name="noiseVarianceEstimateMethod">
		/// Noise variance estimate option.
		//		rmode = 0 ==> Point estimate of noise variance.
		//		rmode = 1 ==> Noise estimates smoothed within segments.
		/// </param>
		/// <returns>A SegmentationResults instance which contains the algorithm output of binary events, segmented log, filtered log, et cetera.</returns>
		public static SegmentationResults Segment(List<double> signal, double threshold, int jumpSequenceWindowSize, int noiseVarianceWindowSize, NoiseVarianceEstimateMethod noiseVarianceEstimateMethod)
		{
			return Segment(signal, threshold, jumpSequenceWindowSize, noiseVarianceWindowSize, noiseVarianceEstimateMethod, 300);
		}

		/// <summary>
		/// Segment a signal using the Maximum Likelihood Estimation of Radhakrishnan, et al, 1991.  Attempts to identify regions of the signal that are
		/// considered "consistent."  Assumes a signal that has a state which changes only at segment boundaries.  The state change can be random and the
		/// noise on top of the signal is modeled as Gaussian, but not necessarily stationary.
		/// </summary>
		/// <param name="signal">Input signal to be segmented.</param>
		/// <param name="threshold">Segmentation threshold.</param>
		/// <param name="jumpSequenceWindowSize">Length of the moving average window sized used for smoothing the input well log to arrive at an initial estimate of the jump sequence variance.</param>
		/// <param name="noiseVarianceWindowSize">Length of the moving average window used for smoothing the noise variances.</param>
		/// <param name="noiseVarianceEstimateMethod">
		/// Noise variance estimate option.
		//		rmode = 0 ==> Point estimate of noise variance.
		//		rmode = 1 ==> Noise estimates smoothed within segments.
		/// </param>
		/// <param name="maxSMLRIterations">Upper bound on the number of Single Most Likelihood Replacement iterations.</param>
		/// <returns>A SegmentationResults instance which contains the algorithm output of binary events, segmented log, filtered log, et cetera.</returns>
		public static SegmentationResults Segment(List<double> signal, double threshold, int jumpSequenceWindowSize, int noiseVarianceWindowSize, NoiseVarianceEstimateMethod noiseVarianceEstimateMethod, int maxSMLRIterations)
		{
			// Scalars which are need for output from the call to the algorithm.
			int		numberOfBinaryEvents	= 0;
			double	jumpSequenceVariance	= 0;
			double	segmentDensity			= 0;
			int		iterations				= 0;
			int		error					= 0;
	
			// We seem to need to create the array in the function immediately prior to calling the unmanaged code, otherwise the garbage collector
			// gets cute and moves things around, which the unmanaged code takes offense to.
			double[] signalToPass = new double[signal.Count];
			signal.CopyTo(signalToPass, 0);

			// Create the output arrays.  After being populated by the call to the algorithm they are grouped, stored, and returned in the SegmenationResults data structure.
			double[] Q		= new double[signal.Count];
			double[] FLTLOG = new double[signal.Count];
			double[] SEGLOG	= new double[signal.Count];
			double[] R		= new double[signal.Count];

			int size			= signalToPass.Length;
			int estimateMethod	= (int)noiseVarianceEstimateMethod;

			// Function call to the C DLL.
			SegmentSignalWrapper(signalToPass, size, threshold, jumpSequenceWindowSize, noiseVarianceWindowSize, estimateMethod, maxSMLRIterations, Q, out numberOfBinaryEvents, FLTLOG, SEGLOG, R, out jumpSequenceVariance, out segmentDensity, out iterations, out error);

			return new SegmentationResults(Q, numberOfBinaryEvents, FLTLOG, SEGLOG, R, jumpSequenceVariance, segmentDensity, iterations, error);
		}

		#endregion

		#endregion

		#region Post processing

		/// <summary>
		/// Creates a list of indexes which indicate where the "significant zones" are in the results of the segmentation.  A significant
		/// zone is defined as a region between binary events (for example 1,0,0,0,1 is a region/zone of 3) that are longer than the
		/// specified threshold.
		/// 
		/// An example use for this algorithm would to be to first run the segmentation algorithm on the log/signal to get the results, then
		/// use this algorithm to find the regions of interest.
		/// 
		/// Since a log/signal fed to the segmentation algorithm is the dependent values (y values of a plot), the independent (x values)
		/// must be supplied to this algorithm to enable evaluate the "length" of each zone.
		/// </summary>
		/// <param name="binaryEvents">Array of binary events (1s and 0s) which specify the boundaries of each zone.</param>
		/// <param name="xData">Independent values associated with the log/signal fed to the Segment algorithm and produced the SegmentationResults.</param>
		/// <param name="threshold">Length a zone must be to be consider "significant."  Zones shorter than this are ignored.</param>
		/// <returns>A List of arrays of length 2 indicating the zones.  Each entry in the List is a pair indicating the starting and ending index of one significant zone.</returns>
		public static List<int[]> FindSignificantZones(double[] binaryEvents, double[] xData, double threshold)
		{
			return FindSignificantZones(binaryEvents, xData, threshold, false);
		}

		/// <summary>
		/// Creates a list of indexes which indicate where the "significant zones" are in the results of the segmentation.  A significant
		/// zone is defined as a region between binary events (for example 1,0,0,0,1 is a region/zone of 3) that are longer than the
		/// specified threshold.
		/// 
		/// An example use for this algorithm would to be to first run the segmentation algorithm on the log/signal to get the results, then
		/// use this algorithm to find the regions of interest.
		/// 
		/// Since a log/signal fed to the segmentation algorithm is the dependent values (y values of a plot), the independent (x values)
		/// must be supplied to this algorithm to enable evaluate the "length" of each zone.
		/// </summary>
		/// <param name="binaryEvents">Array of binary events (1s and 0s) which specify the boundaries of each zone.</param>
		/// <param name="xData">Independent values associated with the log/signal fed to the Segment algorithm and produced the SegmentationResults.</param>
		/// <param name="threshold">Length a zone must be to be consider "significant."  Zones shorter than this are ignored.</param>
		/// <param name="includeBoundries">If true, the end data points are added as part of the zone.  Does not change how zones are found.</param>
		/// <returns>A List of arrays of length 2 indicating the zones.  Each entry in the List is a pair indicating the starting and ending index of one significant zone.</returns>
		public static List<int[]> FindSignificantZones(double[] binaryEvents, double[] xData, double threshold, bool includeBoundries)
		{
			List<int[]> significantZones = new List<int[]>();

			if (xData.Length != binaryEvents.Length)
			{
				throw new ArgumentException("The length of the data does not match the length of the binary events.");
			}

			// Find the first "0" entry which will mark the beginning of a zone.
			int zoneStart = FindNextZero(binaryEvents, 0);

			for (int currentIndex = zoneStart+1; currentIndex < binaryEvents.Length; currentIndex++)
			{
				if (binaryEvents[currentIndex] == 1)
				{
					CheckIfValidZone(xData, threshold, includeBoundries, significantZones, zoneStart, currentIndex-1);

					// Scan for the start of the next potential section.
					zoneStart = FindNextZero(binaryEvents, currentIndex);
					currentIndex = zoneStart;
				}
			}

			// If the last entry of the binary events is zero, we need to handle the final section.
			if (binaryEvents[binaryEvents.Length-1] == 0)
			{
				CheckIfValidZone(xData, threshold, includeBoundries, significantZones, zoneStart, binaryEvents.Length-1);
			}

			// When including the boundaries (includeBoundaries==true), it is possible to have added data points before the beginning of the array
			// and after the end.  We need to check for and, if they exist, correct them.
			if (significantZones.Count > 0)
			{
				if (significantZones[0][0] < 0)
				{
					significantZones[0][0] = 0;
				}

				if (significantZones[significantZones.Count-1][1] >= binaryEvents.Length)
				{
					significantZones[significantZones.Count-1][1] = binaryEvents.Length-1;
				}
			}

			return significantZones;
		}

		/// <summary>
		/// Scans the binary events for the next zero.  Helper function to FindSignificantZones.
		/// </summary>
		/// <param name="binaryEvents">Binary event sequence array.</param>
		/// <param name="currentIndex">Index to start scanning from.</param>
		/// <returns>Index of next zero.</returns>
		private static int FindNextZero(double[] binaryEvents, int currentIndex)
		{
			// Find the first zero.
			while (binaryEvents[currentIndex] == 1 && currentIndex < binaryEvents.Length-1)
			{
				currentIndex++;
			}
			return currentIndex;
		}

		/// <summary>
		/// Checks to see if the threshold for being a significant zone is met.  If it is, it is added to the list of significant zones.
		/// </summary>
		/// <param name="xData">Independent values associated with the log/signal fed to the Segment algorithm and produced the SegmentationResults.</param>
		/// <param name="threshold">Length a zone must be to be consider "significant."  Zones shorter than this are ignored.</param>
		/// <param name="includeBoundries">If true, the end data points are added as part of the zone.  Does not change how zones are found.</param>
		/// <param name="significantZones">List of significant zones.</param>
		/// <param name="startIndex">Starting index of zone.</param>
		/// <param name="endIndex">Ending index of the zone.</param>
		private static void CheckIfValidZone(double[] xData, double threshold, bool includeBoundries, List<int[]> significantZones, int startIndex, int endIndex)
		{
			// Check to see if the threshold is met.  If it met, we will store the section
			// as a significant zone.  If the threshold is not met, then the zone is not long
			// enough so it is ignored and we scan ahead.
			if (Math.Abs(xData[endIndex] - xData[startIndex]) > threshold)
			{
				if (includeBoundries)
				{
					significantZones.Add(new int[] {startIndex-1, endIndex+1});
				}
				else
				{
					significantZones.Add(new int[] {startIndex, endIndex});
				}
			}
		}

		#endregion

	} // End class.
} // End namespace.