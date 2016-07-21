#include <stdlib.h>
#include <math.h>
#include <stdio.h>
// plinkBed, the file name of plink bed file
// NumSample, Number of Samples in Plink
// NumSNP, Number of SNPs in plink
// distMat, distance matrix NumSample * NumSample
// E, environment vector with length = NumSample
// result, 12 * NSNP, including
// 1:5		#0, #1, #2, #NA MAF of G
// 6:10		#0, #1, #2, #NA MAF of GE
// 11		SM
// 12		SI
void parsePlink2 (const char **plinkBed, int *NumSample, int *NumSNP, double *distMat, int *E, double *result){
	int NSam = *NumSample;
	int NSNP = *NumSNP;
	int *Gmap = (int *) malloc (4 * 256 * sizeof(int));
	int *G = (int *) malloc (NSam * sizeof(int));
	// int *GE = (int *) malloc (NSam * sizeof(int));
	unsigned char buffer; // note: 1 byte
	int tmp;
	int sampleOffSet = NSam % 4;
	int i, j, k;
	int iResult;
	int GDiff;
	double dExp = 0;
	int rowResult = 12;
	
	//initialize the result to all 0s
	
	for(i = 0; i < NSNP; i++)
		for(j = 0; j < rowResult; j++)
			result[i*rowResult + j] = 0;
	
	// minus mean from dExp
	for(i = 0; i < NSam - 1; i++){
		for(j = i + 1; j < NSam; j++){
			dExp += distMat[i*NSam+j];
		}
	}
	
	dExp = dExp / (((double)NSam) * (NSam-1) / 2);
	
	for(i = 0; i < NSam - 1; i++){
		for(j = i + 1; j < NSam; j++){
			distMat[i*NSam+j] = distMat[i*NSam+j] - dExp;
			distMat[j*NSam+i] = distMat[j*NSam+i] - dExp;
		}
	}
	
	// creat a map for a single byte to 4 genotype in the same order with plink.fam file
	// counts are the number of Allele A1 in bim file
	for(i = 0; i < 256; i++){
		buffer = (unsigned char) i;
		k = i * 4;
		for(j = 0; j < 4; j++){
			tmp = (buffer >> (j * 2)) & 3;
			switch(tmp){
				case 0:
					Gmap[k + j] = 2;
					break;
				case 2:
					Gmap[k + j] = 1;
					break;
				case 3:
					Gmap[k + j] = 0;
					break;
				default:
					Gmap[k + j] = -9;
			}
		}
	}
	

	FILE *ptr_plink = fopen(*plinkBed, "rb");
	if(!ptr_plink){
		printf("Unable to open file!");
	}
	fseek(ptr_plink, 3, SEEK_SET);
	
	for(j = 0; j < NSNP; j++){
		// fseek(ptr_plink, 3 + (NSam/4+1) * j, SEEK_SET);
		iResult = j * rowResult;
		for(i = 0; i < NSam - 3; i=i+4){
			fread(&buffer, 1, 1, ptr_plink);
			// tmp = tmp * 4
			tmp = (((int) buffer) << 2);
			for(k = 0; k < 4; k++){
				G[i+k] = Gmap[tmp + k];
				switch(G[i+k]){
					case 0:
						result[iResult] = result[iResult] + 1;
						break;
					case 1:
						result[iResult + 1] = result[iResult + 1] + 1;
						break;
					case 2:
						result[iResult + 2] = result[iResult + 2] + 1;
						break;
					default:
						result[iResult + 3] = result[iResult + 3] + 1;
				}
				
				// switch(E[i+k]){
				// 	case 0:
				// 		GE[i+k] = (G[i+k] == -9 ? -9 : 0);
				// 		break;
				// 	case 1:
				// 		GE[i+k] = G[i+k];
				// 		break;
				// 	default:
				// 		GE[i+k] = -9;
				// }
				//
				// switch(GE[i+k]){
				// 	case 0:
				// 		result[iResult + 5] = result[iResult + 5] + 1;
				// 		break;
				// 	case 1:
				// 		result[iResult + 6] = result[iResult + 6] + 1;
				// 		break;
				// 	case 2:
				// 		result[iResult + 7] = result[iResult + 7] + 1;
				// 		break;
				// 	default:
				// 		result[iResult + 8] = result[iResult + 8] + 1;
				// }
			}
		}
		if(sampleOffSet > 0){
			fread(&buffer, 1, 1, ptr_plink);
			// tmp = tmp * 4
			tmp = (((int) buffer) << 2);
			for(k = 0; k < sampleOffSet; k++){
				G[i+k] = Gmap[tmp + k];
				switch(G[i+k]){
					case 0:
						result[iResult] = result[iResult] + 1;
						break;
					case 1:
						result[iResult + 1] = result[iResult + 1] + 1;
						break;
					case 2:
						result[iResult + 2] = result[iResult + 2] + 1;
						break;
					default:
						result[iResult + 3] = result[iResult + 3] + 1;
				}
				
				// switch(E[i+k]){
				// 	case 0:
				// 		GE[i+k] = (G[i+k] == -9 ? -9 : 0);
				// 		break;
				// 	case 1:
				// 		GE[i+k] = G[i+k];
				// 		break;
				// 	default:
				// 		GE[i+k] = -9;
				// }
				//
				// switch(GE[i+k]){
				// 	case 0:
				// 		result[iResult + 5] = result[iResult + 5] + 1;
				// 		break;
				// 	case 1:
				// 		result[iResult + 6] = result[iResult + 6] + 1;
				// 		break;
				// 	case 2:
				// 		result[iResult + 7] = result[iResult + 7] + 1;
				// 		break;
				// 	default:
				// 		result[iResult + 8] = result[iResult + 8] + 1;
				// }
			}
		}
		
		result[iResult + 4] = (result[iResult + 1] + result[iResult + 2] + result[iResult + 2]) / (NSam + NSam - result[iResult + 3] - result[iResult + 3]);
		// result[iResult + 9] = (result[iResult + 6] + result[iResult + 7] + result[iResult + 7]) / (NSam + NSam - result[iResult + 8] - result[iResult + 8]);
		
		for(i = 0; i < NSam - 1; i++){
			for(k = i + 1; k < NSam; k++){
				if((G[i] != -9) && (G[k] != -9)){
					GDiff = abs(G[i] - G[k]);
					// if(GDiff > 0){
					// 	result[iResult + 10] += distMat[i*NSam + k];
					// 	if(GDiff > 1)
					// 		result[iResult + 10] += distMat[i*NSam + k];
					// }
					switch(GDiff){
						case 0:
							break;
						case 2:
							result[iResult + 10] += distMat[i*NSam + k];
						default:
							result[iResult + 10] += distMat[i*NSam + k];
					}
				}
				
				// if((GE[i] != -9) && (GE[k] != -9)){
				// 	GDiff = abs(GE[i] - GE[k]);
				// 	// if(GDiff > 0){
				// 	// 	result[iResult + 11] += distMat[i*NSam + k];
				// 	// 	if(GDiff > 1)
				// 	// 		result[iResult + 11] += distMat[i*NSam + k];
				// 	// }
				// 	switch(GDiff){
				// 		case 0:
				// 			break;
				// 		case 2:
				// 			result[iResult + 11] += distMat[i*NSam + k];
				// 		default:
				// 			result[iResult + 11] += distMat[i*NSam + k];
				// 	}
				// }
				
			}
		}
	}
	
	
	fclose(ptr_plink);
	free(G);
	// free(GE);
	free(Gmap);
}
