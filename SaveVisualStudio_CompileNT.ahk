; Save & Compile


SetTitleMatchMode 2

^F12::
IfWinActive, Microsoft Visual Studio
{
SetTitleMatchMode 1
 Send ^s
; ControlSend,, {F5}, Strategy - CompileMeStrat
 ControlSend,, {F5}, Indicator - CompileMe
; ControlSend,, {Enter}, Debug Mode
; Send {Enter}
SetTitleMatchMode 2
}