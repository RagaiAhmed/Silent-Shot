#pragma strict

@script RequireComponent (Light)

public var time:float = 0.5f; //time between on and off
	
function Start () {
	StartCoroutine("Flicker");
}

function Update () {

}
	
function Flicker (){
	while(boolean){
		GetComponent.<Light>().enabled = false;
		yield WaitForSeconds(time);
		GetComponent.<Light>().enabled = true;
		yield WaitForSeconds(time);
	}
}