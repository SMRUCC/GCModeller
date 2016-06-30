#include <stdio.h>
#include "Log Separate.h"

extern "C"
{
	// Export our function to a DLL and handle some pointer/addresses handling.
	__declspec(dllexport) void SegmentSignalWrapper(double LOG[], int NSAMPS, double F, int ORDER, int ORDER1, int RMODE, int NITER, double Q[], int *NQ, double FLTLOG[], double SEGLOG[], double R[], double *C, double *D, int *NACT, int *IER)
	{
		SegmentSignal(LOG, NSAMPS, F, ORDER, ORDER1, RMODE, NITER, Q, *NQ, FLTLOG, SEGLOG, R, *C, *D, *NACT, *IER);
	}
}