using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
	/// <summary>
	/// Results of the segmentation.
	/// </summary>
	public class SegmentationResults
	{
		#region Members

		private double[]	_binaryEventSequence;
		private int			_numberOfBinaryEvents;
		private double[]	_filteredSignal;
		private double[]	_segmentedLog;
		private double[]	_noiseVariance;
		private double		_jumpSequenceVariance;
		private double		_segmentDensity;
		private int			_iterations;
		private int			_error;

		#endregion

		#region Construction

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="binaryEventSequence">Binary event sequence.</param>
		/// <param name="numberOfBinaryEvents">Number of binary events found (number of "1"s found in binary event sequence).</param>
		/// <param name="filteredSignal">Filtered signal.</param>
		/// <param name="segmentedLog">Segmented log.</param>
		/// <param name="noiseVariance">Noise variance.</param>
		/// <param name="jumpSequenceVariance">Jump sequence variance.</param>
		/// <param name="segmentDensity">Segment density (ratio of events to total entries in binary event sequence).</param>
		/// <param name="iterations">Number of iterations performed.</param>
		/// <param name="error">Error flag.</param>
		public SegmentationResults(double[] binaryEventSequence, int numberOfBinaryEvents, double[] filteredSignal, double[] segmentedLog, double[] noiseVariance, double jumpSequenceVariance, double segmentDensity, int iterations, int error)
		{
			_binaryEventSequence	= binaryEventSequence;
			_numberOfBinaryEvents	= numberOfBinaryEvents;
			_filteredSignal			= filteredSignal;
			_segmentedLog			= segmentedLog;
			_noiseVariance			= noiseVariance;

			_jumpSequenceVariance	= jumpSequenceVariance;
			_segmentDensity			= segmentDensity;
			_iterations				= iterations;
			_error					= error;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Array that contains 1s at segmented log boundaries and 0s elsewhere.
		/// </summary>
		public double[] BinaryEventSequence
		{
			get
			{
				return _binaryEventSequence;
			}
		}

		/// <summary>
		/// Number of binary event sequences detected (number of "1"s in the BinaryEventSequence array).
		/// </summary>
		public int NumberOfBinaryEvents
		{
			get
			{
				return _numberOfBinaryEvents;
			}
		}

		/// <summary>
		/// Filtered estimate of the signal.
		/// </summary>
		public double[] FilteredSignal
		{
			get
			{
				return _filteredSignal;
			}
		}

		/// <summary>
		/// Average of filter log for each segment.
		/// </summary>
		public double[] SegmentedLog
		{
			get
			{
				return _segmentedLog;
			}
		}

		/// <summary>
		/// Estimate noise variance.
		/// </summary>
		public double[] NoiseVariance
		{
			get
			{
				return _noiseVariance;
			}
		}

		/// <summary>
		/// Estimated variance of the jump sequence.
		/// </summary>
		public double JumpSequenceVariance
		{
			get
			{
				return _jumpSequenceVariance;
			}
		}

		/// <summary>
		/// Estimate segment density.
		/// </summary>
		public double SegmentDensity
		{
			get
			{
				return _segmentDensity;
			}
		}
		
		/// <summary>
		/// SMLR iteration count.
		/// </summary>
		public int Iterations
		{
			get
			{
				return _iterations;
			}
		}

		/// <summary>
		/// Error flag.
		///		Zero - no error.
		///		Less than zero - invalid event density estimated after threshold.  Reduce/increase
		///			threshold and rerun.
		///		Greater than zero - logarithm argument became zero at sample number returned in error during
		///			the calculation of likelihood ratios in SMLR.  There may be more samples of this type which
		///			may give rise to this problem.  Edit/rescale data values and rerun.
		/// </summary>
		public int Error
		{
			get
			{
				return _error;
			}
		}

		#endregion

	} // End class.
} // End namespace.
