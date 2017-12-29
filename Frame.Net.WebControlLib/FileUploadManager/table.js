			var lastClickTR=null;
			var lastTRStyle=null;
			function installDataTable() {
                var allTables=$("body").find("table");
				for(var i=0;i<allTables.length;i++) {
					if($(allTables[i]).hasClass("dataTable")) {
						$(allTables[i]).attr('cellspacing','0');
						$(allTables[i]).attr('cellpadding','0');
						$(allTables[i]).attr('width','100%');
						$(allTables[i]).attr('border-bottom','1px solid #96b1c6;');
			
						var allTRs=$(allTables[i]).find("tr");
			
						if(allTRs!=null && allTRs.length>0) {
							var allTDs;
							for(j=0;j<allTRs.length;j++) {
								if (j == 0) {
									var allThs = $(allTRs[0]).find("th");
									$(allThs[0]).addClass("left");
									$(allThs[allThs.length - 1]).addClass("right");
								} else {
									var allTds = $(allTRs[j]).find("td");
									$(allTds[0]).addClass("leftline");
									if(j%2==1) {
										$(allTRs[j]).addClass("dataTableOdd");
									} else {
										$(allTRs[j]).addClass("dataTableEven");
									}
			
									$(allTRs[j]).bind("mouseover",function() {
										if(lastClickTR!=null) {
											$(lastClickTR).removeClass("dataTableSelected");
											$(lastClickTR).addClass(lastTRStyle);
										}
										lastClickTR=this;
										lastTRStyle=$(lastClickTR).hasClass("dataTableEven") ? "dataTableEven" : "dataTableOdd";
										$(this).addClass("dataTableSelected");
									});
								}
							}
						}
					}
				}
			}