/* GR-CITRUS Milkcocoa Template E1.00 */

#include <Arduino.h>
#include "ESP8266.h"
#include "Milkcocoa.h"
#include "Client_ESP8266.h"
#include "OsakanaFft.h"
#include "PeakDetectMachine.h"
#include "EdgeDetector.h"

#define ESP_Serial      Serial3
#define WLAN_SSID       ""
#define WLAN_PASS       ""
#define MILKCOCOA_APP_ID      ""
#define MILKCOCOA_DATASTORE   ""
#define MILKCOCOA_SERVERPORT  1883

//////////////////////////////////////////////////////////////////////
//	MILK COCOA Definition/Declarations
//////////////////////////////////////////////////////////////////////
ESP8266Client wifi;
const char MQTT_SERVER[] PROGMEM    = MILKCOCOA_APP_ID ".mlkcca.com";
const char MQTT_CLIENTID[] PROGMEM  = __TIME__ MILKCOCOA_APP_ID;
Milkcocoa milkcocoa = Milkcocoa(&wifi, MQTT_SERVER, MILKCOCOA_SERVERPORT, MILKCOCOA_APP_ID, MQTT_CLIENTID);
static void onpush(DataElement *pelem);

//////////////////////////////////////////////////////////////////////
// OsakanaFFT Definition/Declarations
//////////////////////////////////////////////////////////////////////
// debug
#define LOG_Serial			Serial1
#define LOG_NEWLINE			"\r\n"
#define DEBUG_OUTPUT_NUM    512
#define ELOG				DebugPrint
#define WLOG				DebugPrint
#define ILOG				DebugPrint
//#define DLOG				DebugPrint
#define DLOG

#define N					512		// fft sampling num(last half is 0 pad)
#define LOG2N				9		// log2(N)
#define N2					(N/2)	// sampling num of analog input
#define N_ADC				N2
#define T1024_1024			(45.336000000000006f)	// adc speedd(time to take 1024x1024 samples in sec)
#define T_PER_SAMPLE		(T1024_1024/1024.0f/1024.0f)	// factor to compute index to freq
#define FREQ_PER_SAMPLE		((float)(1.0f/T_PER_SAMPLE))
#define MIN_AMPLITUDE		(30.0f/1024.0f)

typedef struct PitchInfo_t {
    float freq;
    uint16_t midiNote;
} PitchInfo_t;

static void ReadData();
static int DetectPitch(OsakanaFftContext_t* ctx, MachineContext_t* mctx, PitchInfo_t* pitchInfo);

osk_complex_t xf[N] = { { 0, 0 } };
float xf2[N2] = { 0 };
float _mf[N2] = { 0 };

void DebugPrint(const char* msg)
{
	LOG_Serial.print(msg);
	LOG_Serial.print(LOG_NEWLINE);
}

void DebugPrint(const float val)
{
	LOG_Serial.print(val);
	LOG_Serial.print(LOG_NEWLINE);
}

void DebugPrint(const osk_complex_t* comps)
{
	for(int i = 0; i < DEBUG_OUTPUT_NUM; i++) {
		LOG_Serial.print(comps[i].re);
		LOG_Serial.print(", ");
		LOG_Serial.print(comps[i].im);
		LOG_Serial.print(LOG_NEWLINE);
	}
}

void DebugPrint(const float* xs) {
	for(int i = 0; i < DEBUG_OUTPUT_NUM; i++) {
		LOG_Serial.print(xs[i]);
		LOG_Serial.print(LOG_NEWLINE);
	}
}

bool setupMilkcocoa()
{
    pinMode(PIN_LED0, OUTPUT);
    digitalWrite(PIN_LED0, HIGH);

    wifi.begin(ESP_Serial, 115200);

    if (!wifi.setOprToStation()) {
        ELOG("to station err\r\n");
       	return false;
    }
    ILOG("to station ok\r\n");
        
    if (!wifi.joinAP(WLAN_SSID, WLAN_PASS)) {
        ELOG("Join AP failure\r\n");
        return false;
    }
    ILOG("Join AP success\r\n");
    ILOG("IP: ");
    ILOG(wifi.getLocalIP().c_str());
        
    if (!wifi.disableMUX()) {
        ELOG("single err\r\n");
        return false;
    }
    ILOG("single ok\r\n");
        
    if(!milkcocoa.on(MILKCOCOA_DATASTORE, "push", onpush)){
        ELOG("milkcocoa on failure");
        return false;
    }
    ILOG("milkcocoa on sucesss");
    return true;
}

bool setupFft(){
	pinMode(PIN_LED0, OUTPUT);
	analogReference(EXTERNAL);
	return true;
}

void setup(){
	Serial1.begin(112500);
	pinMode(PIN_LED0, OUTPUT);
	
	ILOG("GR-CITRUS start");
	digitalWrite(PIN_LED0, HIGH);
	
	digitalWrite(PIN_LED0, HIGH);
	while(!setupMilkcocoa()) {
    	digitalWrite(PIN_LED0, LOW);
		delay(1000);
	}
	
	digitalWrite(PIN_LED0, HIGH);
	while(!setupFft()) {
    	digitalWrite(PIN_LED0, LOW);
		delay(1000);
	}
	digitalWrite(PIN_LED0, HIGH);
}

void loop()
{
	EdgeDetector edge(0);
	MachineContext_t* mctx = NULL;
	mctx = CreatePeakDetectMachineContext();
  
	OsakanaFftContext_t* ctx;
	if (InitOsakanaFft(&ctx, N, LOG2N) != 0) {
		DLOG("InitOsakanaFpFft error");
		exit(1);
	}

	milkcocoa.loop();
	
	while (1) {
		ReadData();
		
		PitchInfo_t pitchInfo;
		memset(&pitchInfo, 0, sizeof(pitchInfo));
		
		int result = DetectPitch(ctx, mctx, &pitchInfo);
		if(result == 0) {
			digitalWrite(PIN_LED0, HIGH);
		} else {
			digitalWrite(PIN_LED0, LOW);\
		}
		
		if(edge.Input(pitchInfo.midiNote)) {
			ILOG("edge!");
	    	DataElement elem = DataElement();
        	elem.setValue("f", pitchInfo.freq);
        	elem.setValue("midi", pitchInfo.midiNote);
        	milkcocoa.push(MILKCOCOA_DATASTORE, &elem);
        	DLOG(elem.toCharArray());
		} else {
			DLOG("no edge");
		}
		{
			milkcocoa.loop();
		}
		ResetMachine(mctx);
	}

	DestroyPeakDetectMachineContext(mctx);

	while(true){delay(1000);}
}

static void onpush(DataElement *pelem) {
	//char tmp[128] = { 0 };
	//snprintf(tmp, sizeof(tmp), "onpush %s %s", pelem->getString("f"), pelem->getString("midi"));
  	//DLOG(tmp);
  	ILOG(pelem->toCharArray());
}

// 1024 samples from analogread to xf[]
static void ReadData()
{
	DLOG("sampling...");
	for(int i = 0; i < N2; i++) {
		int val = analogRead(0);
		xf[i].re = (float)val;
	}
	DLOG("sampled");
}

static int DetectPitch(OsakanaFftContext_t* ctx, MachineContext_t* mctx, PitchInfo_t* pitchInfo)
{
    DLOG("ENT DetectPitch");
    int ret = -1;
	DLOG("raw data --");
	DLOG(xf);

	DLOG("normalizing...");
	for (int i = 0; i < N2; i++) {
		xf[i].re = (xf[i].re - 512.0f)/1024.0f;
		xf[i].im = 0.0f;
		xf[N2 + i].re = 0.0f;
		xf[N2 + i].im = 0.0f;
		xf2[i] = xf[i].re * xf[i].re;
	}
	DLOG("normalized");

	DLOG("-- normalized input signal");
	DLOG(xf);

	DLOG("-- fft/N");
	{
		OsakanaFft(ctx, xf);
	}
	DLOG(xf);

	DLOG("-- power spectrum");
	{
		for (int i = 0; i < N; i++) {
			xf[i].re = xf[i].re * xf[i].re + xf[i].im * xf[i].im;
			xf[i].im = 0.0f;
		}
	}
	DLOG(xf);

	DLOG("-- IFFT");
	OsakanaIfft(ctx, xf);
	DLOG(xf);

	{
		_mf[0] = xf[0].re * 4.0f;
		for (int t = 1; t < N2; t++) {
			_mf[t] = _mf[t - 1] - xf2[t - 1] + xf2[t];
		}
	}
	DLOG("-- ms smart");
	DLOG(_mf);

	// nsdf
	float* _nsdf = _mf; // reuse buffer
	{
		for (int t = 0; t < N2; t++) {
			float mt = _mf[t] + 0.01f; // add small number to avoid 0 div
			_nsdf[t] = xf[t].re / mt;
			_nsdf[t] = _nsdf[t] * 2.0f * 2.0f;
		}
	}
	DLOG("-- _nsdf");
	DLOG(_nsdf);

	DLOG("-- pitch detection");
	{
		for (int i = 0; i < N2; i++) {
			Input(mctx, _nsdf[i]);
		}
	}
	
	PeakInfo_t keyMaximums[1] = { 0 };
	int keyMaxLen = 0;
	GetKeyMaximums(mctx, 0.8f, keyMaximums, 1, &keyMaxLen);
	if (0 < keyMaxLen) {
		float delta = 0;
		char tmp[128] = { 0 };
		if (ParabolicInterp(mctx, keyMaximums[0].index, _nsdf, N2, &delta)) {
			snprintf(tmp, sizeof(tmp), "delta %f\n", delta);
			DLOG(tmp);
		}
		
		float freq = FREQ_PER_SAMPLE / (keyMaximums[0].index + delta);
		const float k = log10f(pow(2.0f, 1.0f / 12.0f));
		uint16_t midi = (uint16_t)round(log10f(freq / 27.5f) / k) + 21;
		
		snprintf(tmp, sizeof(tmp), "freq=%f Hz, note=%s\n", freq, kNoteStrings[midi % 12]);
		ILOG(tmp);
		pitchInfo->freq = freq;
		pitchInfo->midiNote = midi;
		
		ret = 0;
	}

	DLOG("finished");
	return ret;
}
