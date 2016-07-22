#include <stdlib.h>
#include <math.h>
#include <stdio.h>
// Xv is the input dist matrix, Dim*Dim dimension
// result is a vector with length = len, corresponding to the 7 definitions and dmean
// simIdxV is a Dim * nCol matrix, each column is a simulation index vector

void dExp2 (int *Dim, double *Xv, int *nCol, int *simIdxV, int *len, double *result)
{
	int N = *Dim;
	int nSim = *nCol;
	// int n = *len;
	int iSim;
	int ii, jj;
	int i, j, k, l, m, n;
	double tmp;
	
	//Initilize the result vector to all 0
	for(i = 0; i < *len; i++)
		result[i] = 0;
	
	//Calculate mean		
	for(i = 0; i < N - 1; i++){
		for(j = i + 1; j < N; j++){
			result[0] += Xv[i*N+j];
		}
	}
	result[0] = result[0] / (((double)N) * (N-1) / 2);
	// minus mean from Xv
	for(i = 0; i < N - 1; i++){
		for(j = i + 1; j < N; j++){
			Xv[i*N+j] -= result[0];
			Xv[j*N+i] -= result[0];
		}
	}
	
	for(jj = 0; jj < nSim; jj++){
		// ii, ii+1, ii+2, ii+3
		// for(ii = 0; ii + 3 < N; ii = ii + 4)
		for(ii = 0; ii < N - 3; ii = ii + 4){
			iSim = jj * N + ii;
			i = simIdxV[iSim];
			j = simIdxV[iSim + 1];
			k = simIdxV[iSim + 2];
			l = simIdxV[iSim + 3];
			tmp = Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l];
			// result[6] += Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l];
			result[6] += tmp;
			// result[12] += Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l];
			// result[13] += Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] * Xv[k*N+l];
			// result[16] += Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l] * Xv[i*N+l];
			result[12] += Xv[i*N+j] * tmp;
			result[13] += Xv[j*N+k] * tmp;
			result[16] += Xv[i*N+l] * tmp;
			
			tmp = Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l];
			// result[7] += Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l];
			result[7] += tmp;
			// result[14] += Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l];
			// result[15] += Xv[i*N+j] * Xv[j*N+k] * Xv[i*N+k] * Xv[i*N+l];
			result[14] += Xv[i*N+j] * tmp;
			result[15] += Xv[j*N+k] * tmp;
			result[17] += Xv[i*N+j] * Xv[i*N+j] * Xv[k*N+l] * Xv[k*N+l];
			
		}
	}
	tmp = (N / 4) * ((double)nSim);
	result[6] = result[6] / tmp;
	result[7] = result[7] / tmp;
	result[12] = result[12] / tmp;
	result[13] = result[13] / tmp;
	result[14] = result[14] / tmp;
	result[15] = result[15] / tmp;
	result[16] = result[16] / tmp;
	result[17] = result[17] / tmp;
	
	
	for(jj = 0; jj < nSim; jj++){
		// ii, ii+1, ii+2, ii+3, ii + 4
		// for(ii = 0; ii + 4 < N; ii = ii + 5)
		for(ii = 0; ii < N - 4; ii = ii + 5){
			iSim = jj * N + ii;
			i = simIdxV[iSim];
			j = simIdxV[iSim + 1];
			k = simIdxV[iSim + 2];
			l = simIdxV[iSim + 3];
			m = simIdxV[iSim + 4];
			
			result[18] += Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l] * Xv[l*N+m];
			
			tmp = Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l];
			// result[18] += Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l] * Xv[i*N+m];
			result[19] += tmp * Xv[i*N+m];
			// result[19] += Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l] * Xv[l*N+m];
			result[20] += tmp * Xv[l*N+m];
			result[21] += Xv[i*N+j] * Xv[i*N+k] * Xv[l*N+m] * Xv[l*N+m];
			
		}
	}
	tmp = (N / 5) * ((double)nSim);
	result[18] = result[18] / tmp;
	result[19] = result[19] / tmp;
	result[20] = result[20] / tmp;
	result[21] = result[21] / tmp;
	
	for(jj = 0; jj < nSim; jj++){
		// ii, ii+1, ii+2, ii+3, ii + 4, ii + 5
		// for(ii = 0; ii + 5 < N; ii = ii + 6)
		for(ii = 0; ii < N - 5; ii = ii + 6){
			iSim = jj * N + ii;
			i = simIdxV[iSim];
			j = simIdxV[iSim + 1];
			k = simIdxV[iSim + 2];
			l = simIdxV[iSim + 3];
			m = simIdxV[iSim + 4];
			n = simIdxV[iSim + 5];
			
			result[22] += Xv[i*N+j] * Xv[i*N+k] * Xv[l*N+m] * Xv[l*N+n];
			
		}
	}
	tmp = (N / 6) * ((double)nSim);
	result[22] = result[22] / tmp;
}
