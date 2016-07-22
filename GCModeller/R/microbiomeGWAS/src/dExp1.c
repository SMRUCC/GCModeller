#include <stdlib.h>
#include <math.h>
#include <stdio.h>
// Xv is the input matrix, Dim*Dim dimension
// result is a vector with length = len, corresponding to the 7 definitions and dmean

void dExp1 (int *Dim, double *Xv, int *len, double *result)
{
	int N = *Dim;
	// int n = *len;
	// int i, j, k, l;
	int i, j, k;
	double tmp;
	double tmpij, tmpjk, tmpik;

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

	//Minus mean from Xv and calculate Def.1 and Def.3
	for(i = 0; i < N - 1; i++){
		for(j = i + 1; j < N; j++){
			Xv[i*N+j] -= result[0];
			Xv[j*N+i] -= result[0];
			tmp = Xv[i*N+j] * Xv[i*N+j];
			result[1] += tmp;
			tmp = tmp * Xv[i*N+j];
			result[3] += tmp;
			result[8] += tmp * Xv[i*N+j];
		}
	}

	result[1] = result[1] / (((double)N) * (N-1) / 2);
	result[3] = result[3] / (((double)N) * (N-1) / 2);
	result[8] = result[8] / (((double)N) * (N-1) / 2);

	//Calculate Def.2, Def.4 and Def.5
	for(i = 0; i < N - 2; i++){
		for(j = i + 1; j < N - 1; j++){
			for(k = j + 1; k < N; k++){
				result[2] +=	Xv[i*N+j] * Xv[i*N+k] +
								Xv[i*N+j] * Xv[j*N+k] +
								Xv[i*N+k] * Xv[j*N+k];

				tmpij = Xv[i*N+j] * Xv[i*N+j];
				tmpik = Xv[i*N+k] * Xv[i*N+k];
				tmpjk = Xv[j*N+k] * Xv[j*N+k];

				// result[10] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+k] +
				// 				Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] +
				// 				Xv[i*N+k] * Xv[i*N+k] * Xv[j*N+k] * Xv[j*N+k];

				result[10] +=	tmpij * (tmpik + tmpjk) + tmpik * tmpjk;

				// result[4] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] +
				// 				Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+k] +
				// 				Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] +
				// 				Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] +
				// 				Xv[i*N+k] * Xv[i*N+k] * Xv[j*N+k] +
				// 				Xv[i*N+k] * Xv[j*N+k] * Xv[j*N+k];

				// result[4] +=	Xv[i*N+j] * Xv[i*N+j] * (Xv[i*N+k] + Xv[j*N+k]) +
				// 				Xv[i*N+k] * Xv[i*N+k] * (Xv[i*N+j] + Xv[j*N+k]) +
				// 				Xv[j*N+k] * Xv[j*N+k] * (Xv[i*N+j] + Xv[i*N+k]);

				tmpij = tmpij * (Xv[i*N+k] + Xv[j*N+k]);
				tmpik = tmpik * (Xv[i*N+j] + Xv[j*N+k]);
				tmpjk = tmpjk * (Xv[i*N+j] + Xv[i*N+k]);

				result[4] += tmpij + tmpik + tmpjk;

				// result[9] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] +
				// 				Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+k] * Xv[i*N+k] +
				// 				Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] +
				// 				Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] * Xv[j*N+k] +
				// 				Xv[i*N+k] * Xv[i*N+k] * Xv[i*N+k] * Xv[j*N+k] +
				// 				Xv[i*N+k] * Xv[j*N+k] * Xv[j*N+k] * Xv[j*N+k];

				// result[9] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+j] * (Xv[i*N+k] + Xv[j*N+k]) +
				// 				Xv[i*N+k] * Xv[i*N+k] * Xv[i*N+k] * (Xv[i*N+j] + Xv[j*N+k]) +
				// 				Xv[j*N+k] * Xv[j*N+k] * Xv[j*N+k] * (Xv[i*N+j] + Xv[i*N+k]);

				result[9] +=	tmpij * Xv[i*N+j] +
								tmpik * Xv[i*N+k] +
								tmpjk * Xv[j*N+k];

				tmp = Xv[i*N+j] * Xv[j*N+k] * Xv[i*N+k];
				result[5] +=	tmp;
				result[11] +=	tmp * (Xv[i*N+j] + Xv[j*N+k] + Xv[i*N+k]);
			}

		}
	}
	result[2] = result[2] / 3 / (((double)N) * (N-1) * (N-2) / 6);
	result[10] = result[10] / 3 / (((double)N) * (N-1) * (N-2) / 6);
	result[4] = result[4] / 6 / (((double)N) * (N-1) * (N-2) / 6);
	result[9] = result[9] / 6 / (((double)N) * (N-1) * (N-2) / 6);
	result[5] = result[5] / (((double)N) * (N-1) * (N-2) / 6);
	result[11] = result[11] / 3 / (((double)N) * (N-1) * (N-2) / 6);

	// //Calculate Def.6, Def7
	// for(i = 0; i < N - 3; i++){
	// 	for(j = i + 1; j < N - 2; j++){
	// 		for(k = j + 1; k < N - 1; k++){
	// 			for(l = k + 1; l < N; l++){
	// 				result[6] +=	Xv[i*N+j] * Xv[k*N+l] * (Xv[i*N+k] + Xv[i*N+l] + Xv[j*N+k] + Xv[j*N+l]) +
	// 								Xv[i*N+k] * Xv[j*N+l] * (Xv[i*N+j] + Xv[i*N+l] + Xv[k*N+j] + Xv[k*N+l]) +
	// 								Xv[i*N+l] * Xv[j*N+k] * (Xv[i*N+j] + Xv[i*N+k] + Xv[j*N+l] + Xv[k*N+l]);
	//
	// 				// result[7] +=	Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l] +
	// 				// 				Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+l] +
	// 				// 				Xv[i*N+k] * Xv[j*N+k] * Xv[k*N+l] +
	// 				// 				Xv[i*N+l] * Xv[j*N+l] * Xv[k*N+l];
	//
	// 				result[7] +=	Xv[i*N+j] * (Xv[i*N+k] * Xv[i*N+l] + Xv[j*N+k] * Xv[j*N+l]) +
	// 								Xv[k*N+l] * (Xv[i*N+k] * Xv[j*N+k] + Xv[i*N+l] * Xv[j*N+l]);
	//
	// 			}
	// 		}
	//
	// 	}
	// }
	//
	// result[6] = result[6] / 12 / (((double)N) * (N-1) * (N-2) * (N-3) / 24);
	// result[7] = result[7] / 4 / (((double)N) * (N-1) * (N-2) * (N-3) / 24);

}
