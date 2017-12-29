# 简介
EFFC框架是基于多年.net项目开发过程中遇到的问题进行总结汇总而成一个开发框架，从最初的webform开始一直到现在的.net core的版本经历了近8年时间，为本人的一个经验总结，现将其记录于github是并公开<br/>

Efficiency Focus For Core（效率、专注为核心）框架应用于.Net Core第一版发布<br/>
本包为基础库包含EFFC框架的基础接口和基本资源API<br/>

# 框架说明
目前最新的EFFC框架是基于.net core 2.0建构的，里面采用新的模块设计方案，集成了基本的基本的web开发、Rest API、微信开发和企业微信号的开发模块。<br/>
<br/>
<ul>
<li>EFFC框架并非微软的MVC框架的封装，而是基于ISAPI和最新的Middleware重新开发，借助了Razor引擎、v8 Js引擎等技术进行架构的;</li>
<li>EFFC是基于模块化的概念进行构建，在EFFC模块化概念中没有所谓的三层概念，只有每个独立的处理模块，根据应用的需要进行模块组装和层次构建，如：<br/>
基本的Web模块，集成了WebGoModule（用于处理来自客户段的httprequest请求）和BusinessModule，<br/>
而微信模块就集成了WeixinWebGo（用于处理来自微信的request请求）、BusinessModule（进行业务逻辑分层处理）和HttpCallModule（远程访问微信服务接口），
</li>
  <li>在EFFC中Module具备以下特性：<br/>
    可以相互嵌套；<br/>
    只能通过ModuleProxy代理的方式调用，用于输入参数和输出数据格式的转化；<br/>
    Module中的最小执行单位为各种Unit（目前Unit仅用于做数据层的访问处理）；<br/>
    从3.5.1开始，module通过OnUsed方式执行初始化操作，每个Module通过该interface完成自己的初始化操作，以提升runtime时的效能；<br/>
</li>
<li>为了便于习惯于传统三层框架的开发人员开发和理解，在BusinessModule模块中进行了逻辑层（GoLogic）与数据层（Data Unit）的划分，并提供了大量的周边开发工具方法，因此EFFC框架下的最典型特征就是Logic.Action的呼叫请求方式，这是自EFFC 2.0版本就沿用至今的，在基本的Web模块中依然是使用该请求方式在形式上由原来的{logic}.{action}.go改为了/{logic}/{action}
</li>
<li>Data Unit层实际上是Unit模块的定义，在EFFC中Unit原本是用于做Module中最小的执行单元，每个module应该是通过Unit执行内部逻辑的，目前Unit并未实现该方式，而是承担起了数据访问层的功能
</li>
<li>EFFC框架一反传统的model，大量采用dynamic技术构建动态model，以减少开发人员在model编写上的时间，让model具备高扩展性;在EFFC中通过js引擎技术实现动态model与json之间的快速转换操作，不同与NewtonJSON，该方案可以实现普通JSON和js对象与动态Model之间的直接转换，适用面更广，而不必拘泥与严格的JSON格式定义</li>
  <li>通过js引擎技术，在EFFC中衍生出了HostJs的开发方式，在c#代码中可以高效执行js代码，并实现与框架中定义的对象和各种工具类之间的互动，为开发的灵活性提供了一种技术方案（如：自定义开发的小型规则或流程引擎等）</li>
  <li>通过Unit和dynamic model方式，目前EFFC中的数据访问已经完全放弃传统的model定义，目前EFFC的数据访问方式提供了三种方式<br/>
    1.普通sql方式，通过继承DBUnit来编写sql进行数据库的访问，不同于传统的dal编写方式，在DBUnit中只用编写sql，框架会自动根据定义的sql参数格式匹配对应参数,而且可以编写多个sql以便一次性执行<br/>
    2.采用DOD（Data Object Define）方式，该方式是为了增加重用性而建立的，自3.0开始就引入DOD和DOD得动态方式调用，将DBUnit改造，采用对象的概念进行DB访问，如要获取ID为admin的姓名时，直接呼叫DOD.user("admin").name的方式就可以。开发人员只需要在对应的DODUnit中实现对应的属性获取方式即可。（该方式在3.5.1中暂时取消）<br/>
    3.采用JSON方式，该方式源自于mongodb而建立的，面对传统的数据库语言SQL，EFFC框架在3.0中就引入了JSON转SQL的方案，并沿用至今，通过DBExpress，各个数据库访问驱动通过解析JSON转化成各自的SQL方式来访问。该访问方式适用于通过统一的语言方案跨库访问<br/>
 </li>
</ul>

# 使用说明
直接从git上下载对应版本的源码，用vs2015（适用于3.5及以下版本）或vs2017（适用于3.5.1及以上版本）编译即可，在3.5.1及以上的版本中都提供了sample程式，而其它的版本则因历史原因没有提供对应的sample程式

# 历史版本说明
各个历史版本说明如下<br/>
<br/>
1.x版本为最早的webform架构，不上传git<br/>
2.0版本为采用MVC概念，基于ISAPI重构的版本，采用了razor引擎配合logic.action访问方式，建构了基于razor的viewlogic和基于无view的gologic，该版本不上传git<br/>
2.6版本为2.0的bug修正和升级版本，该版本不上传git<br/>
3.0是.net下的最后一个版本，集成了noesis js引擎，添加了DOD访问和JSON访问，该架构应用于多个项目专案<br/>
3.5是将3.0版本框架移植到.net core上，只是个实验版本，未投入实用<br/>
3.5.1是基于.net core 1.1的构建版本<br/>
3.5.2是基于.net core 2.0的构建版本<br/>
