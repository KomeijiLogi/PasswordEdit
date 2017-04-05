function clickEvent() {
    
    document.getElementById('confirmPw').onclick = function () {
        history.go(0);
    }
       
    
   
}
window.onload = function () {
    clickEvent();
}
    


//toast弹出信息
function toast(context) {

    var msg = document.createElement('div');
    msg.innerHTML = context;//提示信息
    msg.style.cssText = "width:60%; min-width:150px; background:#000; opacity:0.5; height:40px; color:#fff; line-height:40px; text-align:center; border-radius:5px; position:fixed; top:40%; left:20%; z-index:999999; font-weight:bold;";
    document.body.appendChild(msg);
    setTimeout(function () {
        var d = 0.5;
        msg.style.webkitTransform = '-webkit-transform ' + d + 's ease-in, opacity ' + d + 's ease-in';
        msg.style.opacity = '0';
        setTimeout(function () {
            document.body.removeChild(msg);
        }, d * 2000);
    }, 3000);

}
