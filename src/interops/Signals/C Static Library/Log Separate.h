#ifndef LOGSEPARATE_H
#define LOGSEPARATE_H

void WrapperTest(double ARRAY[], int NSAMPS);

//                 In                                                                               Out
void SegmentSignal(double LOG[], int NSAMPS, double F, int ORDER, int ORDER1, int RMODE, int NITER, double Q[], int &NQ, double FLTLOG[], double SEGLOG[], double R[], double &C, double &D, int &NACT, int &IER);

//                                   In                                                                  Out
void SingleMostLikelihoodReplacement(double G[], double S[], double C, int NSAMPS, double D, double Q[], bool &CONV, int &IER);

//             In                                               Out
void Threshold(double JMPSEQ[], double C, double F, int NSAMPS, double Q[], double &D);

//                 In                                    Out
void MovingAverage(double ARR1[], int NSAMPS, int ORDER, double ARR2[]);

//                In                                                          Out
void KalmanFilter(double LOG[], double Q[], double C, double R[], int NSAMPS, double GAIN[], double ZTLT[], double ETA[], double STATE[]);

//                                In                                In/Out
void FixedIntervalOptimalSmoother(double Q[], int NSAMPS, double C, double ARR1[], double ARR2[], double ARR3[]);

//                        In                                    Out
void AverageArraySegments(double ARR[], double Q[], int NSAMPS, double AVGOUT[]);

//                                    In                                               Intermediate    Out
void DeglitchAndEstimateNoiseVariance(double LOG[], double Q[], int NSAMPS, int RMODE, double WORK1[], double R[]);

//              In												  Out
void GetSegment(double LOG[], double Q[], int NSAMPS, int SEGSTR, int &SEGEND, int &SEGLEN, double &SEGAVG, bool &endofarray);

//            In          In/Out
void Deglitch(int NSAMPS, double Q[]);

//                 In                                  Out
void GetSpikeyZone(double Q[], int NSAMPS, int SEGSTR, int &SEGEND, bool &endofarray);

//                        In                      Out
void EstimateEventDensity(double Q[], int NSAMPS, double &D, int &NQ);

//                                  In                        Intermediate    Out
void EstimateVarianceOfJumpSequence(double LOG[], int NSAMPS, double WORK1[], double &C);

//                            In                                    Out
void CalculateMeanAndVariance(double ARRAY[], int NSAMPS, int MODE, double &MEAN, double &VAR);

#endif