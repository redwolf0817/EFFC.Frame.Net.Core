// 点击返回顶部按钮，页面滚动到顶部
if(document.querySelector(".toTop")){
    document.querySelector(".toTop").addEventListener("click",function(){
        //当前位置
        var currentPosition=document.documentElement.scrollTop || window.pageYOffset || document.body.scrollTop,
            //设置定时器
            timer=setInterval(function(){
                if(currentPosition>0)
                {
                    currentPosition-=50;
                    window.scrollTo(0,currentPosition);
                }else
                {
                    window.scrollTo(0,0);
                    clearInterval(timer);
                }
            },20);
    })
}


// 滚动到一定的位置，加载相应的部分
window.onscroll=function(){
    var currentPosition=document.documentElement.scrollTop || window.pageYOffset || document.body.scrollTop,
        toTopEl=document.querySelector(".toTop");
    if(currentPosition>=300){
        toTopEl.style.display="block";
    }else{
        toTopEl.style.display="none";
    }
}