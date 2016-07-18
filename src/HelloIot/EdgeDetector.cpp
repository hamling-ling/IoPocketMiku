#include "EdgeDetector.h"


EdgeDetector::EdgeDetector(int val) : _history({0}), _lastNotifiedVal(0)
{
}

EdgeDetector::~EdgeDetector()
{
}

bool EdgeDetector::Input(uint16_t val)
{
	_history[0] = _history[1];
	_history[1] = _history[2];
	_history[2] = val;
	if(_history[1] == _history[2]) {
	    if(_history[0] != _history[1]) {
	        if(_history[1] != _lastNotifiedVal) {
	            _lastNotifiedVal = val;
	            return true;
	        }
	    }
	}
	return false;
}
