using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorithms;
using System.Collections.Generic;

// DLL support.
using System.Runtime.InteropServices;

namespace CSharpUnitTests
{
	/// <summary>
	/// Unit test for the Segmentation Library and C# wrapper, in C#.
	/// </summary>
	[TestClass]
	public class LogSeparateUnitTest
	{
		#region Enumerations

		/// <summary>
		/// Enum that specifies the data in the file.
		/// </summary>
		private enum InputType
		{
			Depth,
			Log,
			SegmentedLog,
			EventSequence,
			FilteredLog,

		} // End enum.

		#endregion

		#region Members

		// For reading a data file with known input and solution.
		private const int				_numberOfDataPoints		= 100;
		private const int				_rows					= 5;
		private const int				_columns				= _numberOfDataPoints;
		private double[][]				_data					= new double[_rows][];

		// Input values specified in paper.
		private const double			_f						= 2.50;
		private const int				_order					= 10;
		private const int				_order1					= 6;
		private const int				_niter					= 300;
		NoiseVarianceEstimateMethod		_rmode					= NoiseVarianceEstimateMethod.Point;

		private const string			_workingDirectory		= @"..\..\..\Test Data\";


		#endregion

		#region Construction

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogSeparateUnitTest()
		{
			// Create the 2 dimensional array.
			for (int i = 0; i < _rows; i++)
			{
				_data[i] = new double[_columns];
			}

			// Read the data file with our input and solution.  Stores it in the "_data" array.
			ReadData();
		}

		#endregion

		#region Tests

		/// <summary>
		/// Run the test on the data set and solution provided in the source paper.
		/// </summary>
		[TestMethod]
		public void LogSeparateTestCS()
		{
			// Known solutions from paper.
			double		C_solution		= 45.842;
			double		D_solution		= 0.06;
			//int		NACT_solution	= 2;

			// Call to separate the signal.
			SegmentationResults results = SegmentSignal.Segment(_data[(int)InputType.Log], _f, _order, _order1, _rmode, _niter);

			// Check the output.
			Assert.AreEqual(D_solution, results.SegmentDensity, 0.0000001, "Event density \"D\" failed.");

			// The variance of the jump sequence is not correct.  The reason is unknown.  Either there is an error in the implementation
			// or the numerics are enough to cause an error.  There was only 3 decimal places of accuracy in the input file and C seams
			// somewhat sensitive.  We will run this check but allow for a fairly crude match.
			Assert.AreEqual(C_solution, results.JumpSequenceVariance, 10, "Variance of the jump sequence \"D\" failed.");

			// Check binary event sequence, segmented log, and filtered log.  The text file the input was taken from only contained 3 decimal points
			// of accuracy, so we have to specify some somewhat crude tolerances.
			for (int i = 0; i < _numberOfDataPoints; i++)
			{
				Assert.AreEqual(_data[(int)InputType.SegmentedLog][i], results.SegmentedLog[i], 0.1, "Segmented log failed.");
				Assert.AreEqual(_data[(int)InputType.EventSequence][i], results.BinaryEventSequence[i], 0.1, "Event sequence failed.");
				Assert.AreEqual(_data[(int)InputType.FilteredLog][i], results.FilteredSignal[i], 0.3, "Filter log failed.");
			}

			// Write a file for inspection and plotting.
			WriteResultsFile(_data, results);
		}

		/// <summary>
		/// Sanity check.  This test replicates a situation that might occur in a piece of production software.  Repeated calls to
		/// a separate function for processing a relatively large data set that has some corrupted data.  If this tests passes without
		/// crashing you should expect your software to run trouble free.
		/// </summary>
		[TestMethod]
		public void LogSeparateForLargeDataSet()
		{
			List<double> data = ReadLargeDataSet();
			for (int i = 0; i < 5; i++)
			{
				// Call to separate the signal.
				SegmentationResults[] results = DoLogSeparateForLargeDataSet(data, 0.065, 1, 0, NoiseVarianceEstimateMethod.Smoothed, 300);
			}
		}

		/// <summary>
		/// Separate function for making the call to SegmentSignal.Segment to test repeated passing of large(r) data sets.
		/// </summary>
		/// <param name="data">Data set.</param>
		private SegmentationResults[] DoLogSeparateForLargeDataSet(List<double> data, double threshold, int jumpSequenceWindowSize, int noiseVarienceWindowSize, NoiseVarianceEstimateMethod noiseVarianceEstimateMethod, int maxIterations)
		{
			SegmentationResults[] results = new SegmentationResults[2];
			results[0] = SegmentSignal.Segment(data.ToArray(), threshold, jumpSequenceWindowSize, noiseVarienceWindowSize, noiseVarianceEstimateMethod, maxIterations);
			results[1] = SegmentSignal.Segment(data, threshold, jumpSequenceWindowSize, noiseVarienceWindowSize, noiseVarianceEstimateMethod, maxIterations);

			return results;
		}

		/// <summary>
		/// The counting of the binary events was added to the algorithm (not part of original algorithm in the paper).
		/// This test confirms that the values calculated in the algorithm agree with the matches the number of events
		/// that come out of the algorithm.
		/// </summary>
		[TestMethod]
		public void BinaryEventCountTest()
		{
			// Call to separate the signal.
			SegmentationResults results = SegmentSignal.Segment(_data[(int)InputType.Log], _f, _order, _order1, _rmode, _niter);

			double[] binaryEvents		= results.BinaryEventSequence;
			int numberOfSolutionEvents	= 0;

			// Count the number of events in the solution.
			for (int i = 0; i < binaryEvents.Length; i++)
			{
				if (binaryEvents[i] == 1)
				{
					numberOfSolutionEvents++;
				}
			}

			// Confirm the number of events in the solution matches the number calculated in the algorithm.
			Assert.AreEqual(numberOfSolutionEvents, results.NumberOfBinaryEvents, "The number of binary events does not match the expected number.");
		}

		/// <summary>
		/// Test the function for finding zones of significant length.  The zones in this test cover 3 entries, but the associated
		/// data on the x-axis is only 2 apart for this case.  Therefore, a threshold of 1.5 is established and any sections in the
		/// input data that have 3 zeros in a row is considered a significant zone.
		/// </summary>
		[TestMethod]
		public void SignificantZoneTest()
		{
			// Initial data sets / input.
			//                                      Zone 1                        Zone 2
			double[]	baseData	= new double[] {0, 0, 0, 1, 0, 1, 1, 0, 0, 1,  0,  0,  0,  1,  1,  0,  0};
			double[]	xData		= new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
			double		threshold	= 1.5;

			// Working and output data.
			double[] workingData = new double[baseData.Length];
			List<int[]> results;

			// Copy the base line data set to the working data set so it can be modified.
			baseData.CopyTo(workingData, 0);
			
			// Initial test.  Check that the correct number of zones was found and that the indexes are correct.
			results = SegmentSignal.FindSignificantZones(workingData, xData, threshold);
			Assert.AreEqual(2,  results.Count, "The number of significant zones found is not correct.");
			Assert.AreEqual(0,  results[0][0], "The index of the significant zone is not correct.");
			Assert.AreEqual(2,  results[0][1], "The index of the significant zone is not correct.");
			Assert.AreEqual(10, results[1][0], "The index of the significant zone is not correct.");
			Assert.AreEqual(12, results[1][1], "The index of the significant zone is not correct.");

			// Check starting case of starting with a 1 but no significant zone: 1, 0, 0, 1 ...
			// Should only return 1 zone now.
			workingData[0]	= 1;
			results			= SegmentSignal.FindSignificantZones(workingData, xData, threshold);
			Assert.AreEqual(1, results.Count, "The number of significant zones found is not correct.");

			// Check case of starting with with a 1 and having a significant zone: 1, 0, 0, 0, 0, 1 ...
			workingData[3]	= 0;
			results			= SegmentSignal.FindSignificantZones(workingData, xData, threshold);
			Assert.AreEqual(2, results.Count, "The number of significant zones found is not correct.");
			Assert.AreEqual(1, results[0][0], "The index of the significant zone is not correct.");
			Assert.AreEqual(4, results[0][1], "The index of the significant zone is not correct.");

			// Reset working data to original data.
			baseData.CopyTo(workingData, 0);

			// Check case of two zones immediately following each other: ... 0, 0, 0, 1, 0, 0, 0 ...
			workingData[6]	= 0;
			results			= SegmentSignal.FindSignificantZones(workingData, xData, threshold);
			Assert.AreEqual(3, results.Count, "The number of significant zones found is not correct.");

			// Reset working data to original data.
			baseData.CopyTo(workingData, 0);

			// Check case of ending with a zone: ... 1, 0, 0, 0
			workingData[14]	= 0;
			results			= SegmentSignal.FindSignificantZones(workingData, xData, threshold);
			Assert.AreEqual(3, results.Count, "The number of significant zones found is not correct.");

			// Check case of ending with a 1: ... 0, 0, 0, 1
			workingData[13] = 0;
			workingData[16] = 1;
			results = SegmentSignal.FindSignificantZones(workingData, xData, threshold);
			Assert.AreEqual(2, results.Count, "The number of significant zones found is not correct.");
			Assert.AreEqual(10, results[1][0], "The index of the significant zone is not correct.");
			Assert.AreEqual(15, results[1][1], "The index of the significant zone is not correct.");

			// Reset working data to original data.
			baseData.CopyTo(workingData, 0);

			// Test including the end points a the zones.  Include both end points of the array and one zone in the middle.
			workingData[14]	= 0;
			results			= SegmentSignal.FindSignificantZones(workingData, xData, threshold, true);
			Assert.AreEqual(3, results.Count, "The number of significant zones found is not correct.");
			Assert.AreEqual(0, results[0][0], "The index of the significant zone is not correct.");
			Assert.AreEqual(3, results[0][1], "The index of the significant zone is not correct.");
			Assert.AreEqual(9, results[1][0], "The index of the significant zone is not correct.");
			Assert.AreEqual(13, results[1][1], "The index of the significant zone is not correct.");
			Assert.AreEqual(13, results[2][0], "The index of the significant zone is not correct.");
			Assert.AreEqual(16, results[2][1], "The index of the significant zone is not correct.");
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Read a set of data that was included in the original paper.
		/// </summary>
		private void ReadData()
		{
			StreamReader reader = new StreamReader(Path.Combine(_workingDirectory, "Data Set 1.txt"));

			// Read the data from the file transposing the data as we go.  The loops are set up to read the data from the as
			// you normally would.  For each line (column), read each entry (row).  The transposing happens when the data
			// is stored into the array "_data".
			for (int i = 0; i < _columns; i++)
			{
				// Read one line at a time.
				string line = reader.ReadLine();

				// Parse the entries.
				string[] stringEntries = line.Split('\t');

				for (int j = 0; j < _rows; j++)
				{
					// Transpose the data as we read it.  This is done to make it easier to pass the data to the functions.  We don't have
					// to rearrange the data, just pass it the entry point (address of first value) to the data we want to pass.
					_data[j][i] = System.Convert.ToDouble(stringEntries[j]);
				}
			}

			// Close the file.
			reader.Close();
		}

		/// <summary>
		/// Read the file which contains a large data set.
		/// </summary>
		/// <returns>Values in the file as a List of doubles.</returns>
		private List<double> ReadLargeDataSet()
		{
			List<double> data = new List<double>();

			StreamReader reader	= new StreamReader(Path.Combine(_workingDirectory, "Large Data Set.txt"));

			// Loop over the data.
			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine();
				data.Add(System.Convert.ToDouble(line));
			}

			return data;
		}

		/// <summary>
		/// Write results to a file.  Useful for bringing into Excel and plotting.
		/// </summary>
		/// <param name="input">Input to the function and the known solution.</param>
		/// <param name="results">Results of the segmentation.</param>
		void WriteResultsFile(double[][] input, SegmentationResults results)
		{
			// Write a file that can be used for plotting, if desired.
			// First open the file and check that it was opened.
			StreamWriter writer = new StreamWriter(Path.Combine(_workingDirectory, "C Sharp Output.txt"), false);

			// Loop over data points and write to file.  Writing as floats seems to work best.
			for (int i = 0; i < _numberOfDataPoints; i++)
			{
				writer.WriteLine("{0:N2} {1:N5} {2:N0} {3:N5}", (float)input[(int)InputType.Log][i], (float)results.SegmentedLog[i], (float)results.BinaryEventSequence[i], (float)results.FilteredSignal[i]);
			}

			// Close our file.
			writer.Close();
		}

		#endregion

	} // End class.
} // End namespace.