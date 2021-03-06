#ifndef _PEAKDETECTMACHINECOMMON_H_
#define _PEAKDETECTMACHINECOMMON_H_

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

#define	kKeyMax	16

typedef enum {
	kStateSearchingBell,
	kStateWalkingOnBell,
	kStateEnd,
	kStateNum
} PeakDetectMachineState_t;

typedef enum {
	kEventPosCross,
	kEventNegCross,
	kEventNmlData,
	kEventEndOfData,
	kEventNum
} PeakDetectMachineEvent_t;

static const char* kNoteStrings[] = {
	"C",	"C#",	"D",	"D#",
	"E",	 "F",	"F#",	"G",
	"G#",	"A",	"Bb",	"B"
	};

// N=512
static const uint8_t kNoteTable[] = {
	0,   0,   0,   5,   0,   8,   5,   2,   0,  10,   8,   6,   5,   3,   2,
	1,   0,  11,  10,   9,   8,   7,   6,   5,   5,   4,   3,   3,   2,   1,
	1,   0,   0,  11,  11,  10,  10,   9,   9,   8,   8,   7,   7,   7,   6,
	6,   5,   5,   5,   4,   4,   4,   3,   3,   3,   2,   2,   2,   1,   1,
	1,   1,   0,   0,   0,  11,  11,  11,  11,  10,  10,  10,  10,   9,   9,
	9,   9,   8,   8,   8,   8,   8,   7,   7,   7,   7,   7,   6,   6,   6,
	6,   6,   5,   5,   5,   5,   5,   4,   4,   4,   4,   4,   4,   3,   3,
	3,   3,   3,   3,   2,   2,   2,   2,   2,   2,   2,   1,   1,   1,   1,
	1,   1,   1,   0,   0,   0,   0,   0,   0,   0,  11,  11,  11,  11,  11,
	11,  11,  11,  10,  10,  10,  10,  10,  10,  10,  10,   9,   9,   9,   9,
	9,   9,   9,   9,   8,   8,   8,   8,   8,   8,   8,   8,   8,   8,   7,
	7,   7,   7,   7,   7,   7,   7,   7,   6,   6,   6,   6,   6,   6,   6,
	6,   6,   6,   6,   5,   5,   5,   5,   5,   5,   5,   5,   5,   5,   4,
	4,   4,   4,   4,   4,   4,   4,   4,   4,   4,   4,   3,   3,   3,   3,
	3,   3,   3,   3,   3,   3,   3,   3,   2,   2,   2,   2,   2,   2,   2,
	2,   2,   2,   2,   2,   2,   1,   1,   1,   1,   1,   1,   1,   1,   1,
	1,   1,   1,   1,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
	0,   0,   0,   0,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,
	11,  11,  11,  11,  11,  10,  10,  10,  10,  10,  10,  10,  10,  10,  10,
	10,  10,  10,  10,  10,  10,   9,   9,   9,   9,   9,   9,   9,   9,   9,
	9,   9,   9,   9,   9,   9,   9,   9,   8,   8,   8,   8,   8,   8,   8,
	8,   8,   8,   8,   8,   8,   8,   8,   8,   8,   8,   8,   7,   7,   7,
	7,   7,   7,   7,   7,   7,   7,   7,   7,   7,   7,   7,   7,   7,   7,
	7,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,
	6,   6,   6,   6,   6,   6,   6,   5,   5,   5,   5,   5,   5,   5,   5,
	5,   5,   5,   5,   5,   5,   5,   5,   5,   5,   5,   5,   5,   4,   4,
	4,   4,   4,   4,   4,   4,   4,   4,   4,   4,   4,   4,   4,   4,   4,
	4,   4,   4,   4,   4,   4,   3,   3,   3,   3,   3,   3,   3,   3,   3,
	3,   3,   3,   3,   3,   3,   3,   3,   3,   3,   3,   3,   3,   3,   3,
	3,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,
	2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   1,   1,   1,
	1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,
	1,   1,   1,   1,   1,   1,   1,   1,   1,   0,   0,   0,   0,   0,   0,
	0,   0,   0,   0,   0,   0,   0,   0,   0,  -0,  -0,  -0,  -0,  -0,  -0,
	0,  -0
};

#ifdef __cplusplus
}
#endif /* __cplusplus */


#endif
