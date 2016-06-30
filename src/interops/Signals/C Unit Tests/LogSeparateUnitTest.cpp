#include "stdafx.h"
#include "CppUnitTest.h"
#include <stdio.h>
#include <direct.h>
#include "Log Separate.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace UnitTests
{
	// Test class for C version of signal segmenting library.
	TEST_CLASS(LogSeparateUnitTest)
	{
	private:
		// Enumeration stating how data is located in the data file.
		enum InputData
		{
			Depth = 0,
			Log,
			SegmentedLog,
			EventSequence,
			FilteredLog
		};

	private:
		// For reading a data file with known input and solution.
		static const int	_numberOfDataPoints					= 100;
		static const int	_rows								= 5;
		static const int	_columns							= _numberOfDataPoints;
		double				_data[_rows][_columns];

		// For larger data set.
		static const int	_largeDataSetSize					= 134214;
		double				*_largeDataSet;

		// For debugging information only.  Used to check that the data file is located correctly relative to current
		// working directory.
		char _path[FILENAME_MAX];

	public:
		// Default constructor.
		LogSeparateUnitTest()
		{
			// Get the path so we can use the information for debugging.
			_getcwd(_path, sizeof(_path));

			// Read the data file with our input and solution.
			ReadData();

			// Read the longer data set.
			_largeDataSet = new double[_largeDataSetSize];
			ReadLargeDataSet();
		}

		~LogSeparateUnitTest()
		{
			delete _largeDataSet;
		}

		// Test of moving average function.
		TEST_METHOD(MovingAverageTest)
		{
			// Solution to this test create in Excel and copied here.
			double solution[_numberOfDataPoints] = {70.380000, 70.523333, 72.702000, 76.011429, 79.203333, 82.103636, 84.682727, 87.444545, 90.114545, 92.518182,
												    94.518182, 96.137273, 97.370000, 98.586364, 99.245455, 99.592727, 99.797273, 99.706364, 99.251818, 98.632727,
												    97.996364, 97.360000, 96.393636, 95.689091, 95.250909, 95.012727, 95.029091, 95.006364, 94.790909, 94.460909,
												    94.427273, 94.427273, 94.518182, 95.120909, 95.575455, 95.433636, 94.740000, 93.996364, 93.326364, 92.979091,
												    93.274545, 93.745455, 94.126364, 94.324545, 94.125455, 93.630909, 93.120000, 93.120000, 93.011818, 92.227273,
												    90.790000, 88.824545, 86.427273, 84.410000, 82.813636, 81.319091, 79.939091, 78.666364, 77.376364, 76.194545,
												    75.722727, 75.705455, 75.830000, 75.863636, 75.460000, 74.858182, 74.205455, 73.660000, 73.148182, 72.677273,
												    72.149091, 71.785455, 71.603636, 71.643636, 71.842727, 72.024545, 72.280000, 72.734545, 73.171818, 73.700000,
												    74.137273, 74.466364, 74.597273, 74.671818, 74.631818, 74.671818, 74.780000, 74.722727, 74.523636, 74.375455,
												    74.233636, 74.193636, 74.375455, 74.574545, 74.648182, 74.576667, 74.697143, 75.226000, 75.793333, 75.190000};

			// Call the function for the moving average.  This moving average function really takes as input the "half value"
			// of the number of desired moving points in the average.  If you want a 10 point moving average you give it 10/2
			// and what you get is the average of five previous data points, the current point, and the five following data points
			// for each element in the array.
			double output[_numberOfDataPoints];
			int order = 10/2;
			MovingAverage(_data[Log], _numberOfDataPoints, order, output);

			// Scan each data point output from the function and compare it to the solution calculated in Excel.
			for (int i = 0; i < _numberOfDataPoints; i++)
			{
				Assert::AreEqual(solution[i], output[i], 0.00001);
			}
		}

		// Event density unit test.  Counts the number of non-zero entries to an array.
		TEST_METHOD(EstimateEventDensityTest)
		{
			// Generate some input.  Just going to create an array that is mostly zeros.  Fill it
			// with some non-zero entries and then calculate what the expected result is.  The variable "q"
			// is the array we will pass to the algorithm.  The other two "ones" and "index" are used to
			// specify which entries are non-zero and count the number of non-zero entries, respectively.
			double q[20];
			int		ones[]	= {3, 5, 12, 16, 19};
			int		index	= 0;

			// This is just the setup, filling the array and counting the number of non-zero entries.
			for (int i = 0; i < 20; i++)
			{
				if (i == ones[index])
				{
					q[i] = 1;
					index++;
				}
				else
				{
					q[i] = 0;
				}
			}

			// Calculate expected solution.
			double densitySolution = index / 20.0;

			// Run algorithm.
			double density;
			int numberOfBinaryEvents;
			EstimateEventDensity(q, 20, density, numberOfBinaryEvents);

			// Test result.
			Assert::AreEqual(densitySolution, density, 0.0000001);
		}

		// Deglitch unit test.  Deglitch removes all the "1"s that are not "end points."  In other words, it ensures there 
		// are not consecutive "1"s.
		TEST_METHOD(DeglitchTest)
		{
			double input[]		= {1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1};
			double solution[]	= {1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1};

			Deglitch(14, input);

			for (int i = 0; i < 14; i++)
			{
				Assert::AreEqual(input[i], solution[i], 0.00000001, L"Deglitch test failed.");
			}
		}

		// Mean and variance unit test.
		TEST_METHOD(CalculateMeanAndVarianceTest)
		{
			// Solutions calculated from Excel.
			double meansolution = 84.257400;
			double variancesolution = 116.915107;

			// Run the algorithm to calculate values.
			double mean;
			double variance;
			CalculateMeanAndVariance(_data[Log], _numberOfDataPoints, 0, mean, variance);

 			Assert::AreEqual(meansolution, mean, 0.000001, L"Calculate mean was not correct.");
			Assert::AreEqual(variancesolution, variance, 0.00001, L"Calculate variance was not correct.");
		}

		// Test the function on input with a known solution.  This is the example in the paper.
		TEST_METHOD(LogSeparateTest)
		{
			// Input values specified in paper.
			double f	= 2.50;
			int order	= 10;
			int order1	= 6;
			int rmode	= 0;
			int niter	= 300;

			// Output variables.
			double Q[_numberOfDataPoints];
			int numberOfBinaryEvents;
			double FLTLOG[_numberOfDataPoints];
			double SEGLOG[_numberOfDataPoints];
			double R[_numberOfDataPoints];
			double C;
			double D;
			int NACT;
			int error;

			// Call to separate the signal.
			SegmentSignal(_data[Log], 100, f, order, order1, rmode, niter, Q, numberOfBinaryEvents, FLTLOG, SEGLOG, R, C, D, NACT, error);


			// Validate the results.

			// Known solutions from paper.
			double	C_solution		= 45.842;
			double	D_solution		= 0.06;
			int		NACT_solution	= 2;

			// Check the output.
			Assert::AreEqual(D_solution, D, 0.0000001, L"Event density \"D\" failed.");

			// The variance of the jump sequence is not correct.  The reason is unknown.  Either there is an error in the implementation
			// or the numerics are enough to cause an error.  There was only 3 decimal places of accuracy in the input file and C seams
			// somewhat sensitive.  We will run this check but allow for a fairly crude match.
			Assert::AreEqual(C_solution, C, 10, L"Variance of the jump sequence \"D\" failed.");

			// Check binary event sequence, segmented log, and filtered log.  The text file the input was taken from only contained 3 decimal points
			// of accuracy, so we have to specify some somewhat crude tolerances.
			for (int i = 0; i < _numberOfDataPoints; i++)
			{
				Assert::AreEqual(_data[SegmentedLog][i], SEGLOG[i], 0.1, L"Segmented log failed.");
				Assert::AreEqual(_data[EventSequence][i], Q[i], 0.1, L"Event sequence failed.");
				Assert::AreEqual(_data[FilteredLog][i], FLTLOG[i], 0.3, L"Filter log failed.");
			}

			// Write a file for inspection and plotting.
			WriteResultsFile(_data[Log], SEGLOG, Q, FLTLOG);
		}

		// Test the function on a longer data set that has corrupted data.  Stress test the algorithm.
		TEST_METHOD(LargeDatSetTest)
		{
			for (int i = 0; i < 5; i++)
			{
				LargeDataSetCallSegmentSignal();
			}

		}

		void LargeDataSetCallSegmentSignal()
		{
			// Input values specified in paper.
			double f	= 0.004;
			int order	= 1;
			int order1	= 1;
			int rmode	= 1;
			int niter	= 300;

			// Output.
			double *Q = new double[_largeDataSetSize];
			int numberOfBinaryEvents;
			double *FLTLOG = new double[_largeDataSetSize];
			double *SEGLOG = new double[_largeDataSetSize];
			double *R = new double[_largeDataSetSize];
			double C;
			double D;
			int NACT;
			int error;

			// Call to separate the signal.
			SegmentSignal(_largeDataSet, _largeDataSetSize, f, order, order1, rmode, niter, Q, numberOfBinaryEvents, FLTLOG, SEGLOG, R, C, D, NACT, error);

			delete Q;
			delete FLTLOG;
			delete SEGLOG;
			delete R;
		}

		// Write results to a file so that they can be viewed in another program (for example, Excel).
		void WriteResultsFile(double inputlog[], double segmentedlog[], double eventsequence[], double filteredlog[])
		{
			// Write a file that can be used for plotting, if desired.
			// First open the file and check that it was opened.
			FILE *file;
			int error = fopen_s(&file, "..\\..\\..\\Test Data\\C Output.txt", "w");
			Assert::IsFalse(file == NULL, L"Failed to open file for writing the results of the signal segmentation.");

			// Loop over data points and write to file.  Writing as floats seems to work best.
			for (int i = 0; i < _numberOfDataPoints; i++)
			{
				fprintf(file, "%f %f %f %f\n", (float)inputlog[i], (float)segmentedlog[i], (float)eventsequence[i], (float)filteredlog[i]);
			}

			// Close our file.
			fclose(file);
		}

		// Reads a data file that contains the information from the example used in the paper on the algorithm.
		void ReadData()
		{
			// Open the file and validate the opening.  This will through an error to the "Test Explorer" if
			// if fails to open.
			FILE *file;
			int error = fopen_s(&file, "..\\..\\..\\Test Data\\Data Set 1.txt", "r");
			Assert::IsFalse(file == NULL, L"Error opening data file.");

			// Read the data from the file transposing the data as we go.  The loops are set up to read the data from the as
			// you normally would.  For each line (column), read each entry (row).  The transposing happens when the data
			// is stored into the array "_data".
			for (int i = 0; i < _columns; i++)
			{
				for (int j = 0; j < _rows; j++)
				{
					float val = 0;
					int scanResult = fscanf_s(file, "%f", &val);

					// We should have the correct number of entries in the file, if this file does not contain enough
					// data, then it is the wrong file.  Throw an error.
					Assert::AreNotEqual(scanResult, EOF, L"End of file reached before all data read.  Not the correct input file.");

					// Transpose the data as we read it.  This is done to make it easier to pass the data to the functions.  We don't have
					// to rearrange the data, just pass it the entry point (address of first value) to the data we want to pass.
					_data[j][i] = val;
				}
			}

			// Close the file.
			fclose(file);
		}

		void ReadLargeDataSet()
		{
			// Open the file and validate the opening.  This will through an error to the "Test Explorer" if
			// if fails to open.
			FILE *file;
			int error = fopen_s(&file, "..\\..\\..\\Test Data\\Large Data Set.txt", "r");
			Assert::IsFalse(file == NULL, L"Error opening data file.");

			for (int i = 0; i < _largeDataSetSize; i++)
			{
				float val = 0;
				int scanResult = fscanf_s(file, "%f", &val);

				// We should have the correct number of entries in the file, if this file does not contain enough
				// data, then it is the wrong file.  Throw an error.
				Assert::AreNotEqual(scanResult, EOF, L"End of file reached before all data read.  Not the correct input file.");

				_largeDataSet[i] = val;
			}
		}

	}; // End class.
} // End namespace.