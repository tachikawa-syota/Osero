#pragma strict

function Update () {
    if (Input.GetButtonDown("Fire1")) {
        var screenPoint = Input.mousePosition;      
        screenPoint.z = 10;
        var v = Camera.main.ScreenToWorldPoint(screenPoint);
        var key_x = Mathf.Floor(v.x);
        var key_y = Mathf.Floor(v.z);
        GameObject.FindWithTag("GameController").SendMessage("putPiece", Vector2(key_x, key_y));
    }
}