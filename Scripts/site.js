//日本語版データピッカー
$(function () {
    $('.cal').datepicker({
        changeYear: true,  // 年選択をプルダウン化
        changeMonth: true  // 月選択をプルダウン化
    });
    $.datepicker.regional['ja'] = {
        closeText: '閉じる',
        prevText: '<前',
        nextText: '次>',
        currentText: '今日',
        monthNames: ['1月', '2月', '3月', '4月', '5月', '6月',
            '7月', '8月', '9月', '10月', '11月', '12月'],
        monthNamesShort: ['1月', '2月', '3月', '4月', '5月', '6月',
            '7月', '8月', '9月', '10月', '11月', '12月'],
        dayNames: ['日曜日', '月曜日', '火曜日', '水曜日', '木曜日', '金曜日', '土曜日'],
        dayNamesShort: ['日', '月', '火', '水', '木', '金', '土'],
        dayNamesMin: ['日', '月', '火', '水', '木', '金', '土'],
        weekHeader: '週',
        dateFormat: 'yy年mm月dd日',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: '年',
        yearRange: '-100:+0'
    };
    $.datepicker.setDefaults($.datepicker.regional['ja']);

});

$(function () {
    $('.fadeBtn').hover(function () {
        $(this).animate({ opacity: '0.5', fontSize: '22px' }, 300);
    }, function () {
        $(this).stop().animate({ opacity: '1', fontSize: '18px' }, 300);
    })
    $('.cssAnim').hover(function () {
        $(this).addClass('rotateY');
    }, function () {
        $(this).removeClass('rotateY');
    })
    $('.cssImg').hover(function () {
        $(this).addClass('cssScale');
    }, function () {
        $(this).removeClass('cssScale');
    })

    $('#mddNav > ul >li').hover(function () {
         childPanel = $(this).children('.mddWrap');
        childPanel.each(function () {
            childPanel.css({ height: '0', display: 'block', opacity: '0' }).stop().animate({ height: '200px', opacity: '1' }, 200, 'swing');
        });
    }, function () {
        childPanel.css({ display: 'none' });
    });
    //ツールチップ処理
    $('a:has(.ttpShow)').mouseover(function (e) {
        $('body').append('<div id="ttpPanel"></div>');
        $('#ttpPanel').load("/Home/RoleIndex");
        $('#ttpPanel').css({ top: (e.pageY + 10) + 'px', left: (e.pageX + 10) + 'px' }).fadeIn('fast');
    }).mousemove(function (e) {
        $('#ttpPanel').css({ top: (e.pageY + 10) + 'px', left: (e.pageX + 10) + 'px' });
    }).mouseout(function () {
        $('#ttpPanel').remove();
    });


    $('.required').on('keydown keyup keypress change focus blur', function () {
        if ($(this).val() == ''){
            $(this).css({ backgroundColor: '#ffcccc' });
        } else {
            $(this).css({ backgroundColor: '#fff' });
        }
    }).change();

    $('.inputCount .required').on('keydown keyup keypress change', function () {

    //countThis.find('.inputCount .required').on('keydown keyup keypress change', function () {
        var thisValueLength = $(this).val().length,
            countDown = 120 - thisValueLength;
        console.log('hello');
        $('.count').text(countDown);

    });


    var mdwBtn = $('.modalBtn'),
        overlatyOpacity = 0.7,
        fadeTime = 500;
    mdwBtn.on('click', function (e) {
        e.preventDefault();
        console.log('happy');
        var setMdw = $(this),
            setHref = setMdw.attr('href');
        wdHeight = $(window).height();
        $('body').append('<div id="mdOverlay"></div><div id="mdWindow"><div class="mdClose">×</div><div id="contWrap"></div></div>');
        $("#contWrap").load(setHref);
        //$.ajax({
        //    type: "Post",
        //    url: "/Articles/_Index", /Home/_RoleIndex
        //    success: function (data) {
        //        $("#contWrap").html(data);
        //    },
        //    error: function () {
        //        alert('error!');
        //    }
        //})
        $('#mdOverlay, #mdWindow').css({ display: 'block', opacity: '0' });
        $('#mdOverlay').css({ height: wdHeight }).stop().animate({ opacity: overlatyOpacity }, fadeTime);
        $('#mdWindow').stop().animate({ opacity: '1' }, fadeTime);
        $('#mdOverlay,.mdClose').on('click', function () {
            $('#mdOverlay, #mdWindow').stop().animate({ opacity: '0' }, fadeTime, function () {
                $('#mdOverlay, #mdWindow').remove();
            });
        });
    });
    var mdwiFBtn = $('.modaliFBtn');
    mdwiFBtn.on('click', function (e) {
        e.preventDefault();
        var setMdw = $(this),
            setHref = mdwiFBtn.attr('href');
        wdHeight = $(window).height();
        $('body').append('<div id="mdOverlay"></div><div id="mdWindow"><div class="mdClose">×</div><iframe id="contiFWrap"></div></div>');
        //$('body').append('<div id="mdWindow"><div class="mdMove">■</div><div class="mdClose">×</div><iframe id="contiFWrap"></div></div>');
        $('#contiFWrap').attr('src', setHref);
        $('#mdOverlay, #mdWindow').css({ display: 'block', opacity: '0' });
        $('#mdOverlay').css({ height: wdHeight }).stop().animate({ opacity: overlatyOpacity }, fadeTime);
        $('#mdWindow').stop().animate({ opacity: '1' }, fadeTime);
        $('#mdOverlay,.mdClose').on('click', function () {
            $('#mdOverlay, #mdWindow').stop().animate({ opacity: '0' }, fadeTime, function () {
                $('#mdOverlay, #mdWindow').remove();
            });
        });
        var flgDrug = false,
            windowY = parseInt($('#mdWindow').css('top')),
            windowX = parseInt($('#mdWindow').css('left')),
            winDiffY = parseInt($('.mdMove').css('top')) - windowY,
            winDiffX = parseInt($('.mdMove').css('left')) - windowX;
        console.log('Y = ' + winDiffY);
        console.log('X = ' + winDiffX);
        $('.mdMove').on('mousedown', function () {
            console.log('down:' );
            console.log('Y = ' + $('#mdWindow').css('top'));
            console.log('X = ' + $('#mdWindow').css('left'));
            console.log('eY = ' + $('.mdMove').css('top'));
            console.log('eX = ' + $('.mdMove').css('left'));
            flgDrug = true;
        }).mousemove(function (e) {
            console.log('Catchmove');
            if (flgDrug) {
                console.log('Y = ' + $('#mdWindow').css('top'));
                console.log('X = ' + $('#mdWindow').css('left'));
                console.log('eY = ' + e.pageY);
                console.log('eX = ' + e.pageX);
               console.log('move');
                $('#mdWindow').css({ top: (e.pageY + 15) + 'px', left: (e.pageX + 15) + 'px' });
            }
        }).mouseup(function () {
            flgDrug = false;
        });
    });



    //コメントのモーダル
    var articleBtn = $('#articlModalBtn'),
        overlatyOpacity = 0.7,
        fadeTime = 500;
    articleBtn.on('click', function (e) {
        e.preventDefault();
        wdHeight = $(window).height();
        var id = $('#Id').val();
        $('body').append('<div id="mdOverlay"></div><div id="mdWindow"><div class="mdClose">×</div><div id="contWrap"></div></div>');
//        $("#contWrap").load("/Comments/Index?articleId=" + id);
        $.ajax("/Comments/Index?articleId=" + id).done(data => $("#contWrap").html(data));
        $('#mdOverlay, #mdWindow').css({ display: 'block', opacity: '0' });
        $('#mdOverlay').css({ height: wdHeight }).stop().animate({ opacity: overlatyOpacity }, fadeTime);
        $('#mdWindow').stop().animate({ opacity: '1' }, fadeTime);
        $('#mdOverlay,.mdClose').on('click', function () {
            $('#mdOverlay, #mdWindow').stop().animate({ opacity: '0' }, fadeTime, function () {
                $('#mdOverlay, #mdWindow').remove();
            });
        });


        $('.addComment').on('click', function () {
            var ss = $("#contWrap").find('#addComment');
            console.log('oo');
        });
    });
});

//Ajax モーダル
function modalBefore() {
    $('body').append('<div id="mdOverlay"></div><div id="mdWindow"><div class="mdClose">×</div><div id="contWrap"></div></div>');
}
function modalGo(context) {
    var overlatyOpacity = 0.7,
        wdHeight = $(window).height(),
        fadeTime = 500;
    $('#mdOverlay, #mdWindow').css({ display: 'block', opacity: '0' });
    $('#mdOverlay').css({ height: wdHeight }).stop().animate({ opacity: overlatyOpacity }, fadeTime);
    $('#mdWindow').stop().animate({ opacity: '1' }, fadeTime);
    $('#mdOverlay,.mdClose').on('click', function () {
        $('#mdOverlay, #mdWindow').stop().animate({ opacity: '0' }, fadeTime, function () {
            $('#mdOverlay, #mdWindow').remove();
        });
    });
}



//画像読み込み後の実行の場合こちら
$(window).on('load', function () {
    var thisTicker1 = $('.ticker1'),
        tickerUl1 = thisTicker1.find('ul'),
        tickerLi = tickerUl1.find('li'),
        liFirst = tickerUl1.find('li:first');
    liFirst.css({ display: 'block', opacity: '0', zIndex: '98' })
        .stop().animate({ opacity: '1' }, 1000, 'linear').addClass('showlist');

    setInterval(function () {
        var showLi = thisTicker1.find('.showlist');
        showLi.animate({ opacity: '0' }, 1000, 'linear', function () {
            $(this).next().css({ display: 'block', opacity: '0', zIndex: '99' })
                .animate({ opacity: '1' }, 1000, 'swing').addClass('showlist')
                .end().appendTo(tickerUl1).css({ display: 'none', zIndex: '98' }).removeClass('showlist');
        });
    }, 4000);

    var thisTicker2 = $('.ticker2'),
        tickerUl2 = thisTicker2.find('ul'),
        tickerLi2 = tickerUl2.find('li'),
        liFirst = tickerUl2.find('li:first'),
        listHeight = tickerLi2.height();
    liFirst.css({ top: listHeight, display: 'block', opacity: '0', zIndex: '98' })
        .stop().animate({ top: '0', opacity: '1' }, 500, 'linear').addClass('showlist');

    setInterval(function () {
        var showLi = thisTicker2.find('.showlist');
        showLi.animate({ top: -(listHeight), opacity: '0' }, 500, 'linear')
            .next().css({ top: listHeight, display: 'block', opacity: '0', zIndex: '99' })
            .animate({ top: '0', opacity: '1' }, 500, 'linear').addClass('showlist')
                .end().appendTo(tickerUl2).css({ zIndex: '98' }).removeClass('showlist');
    }, 3500);

    var thisTicker3 = $('.ticker3'),
        tickerUl3 = thisTicker3.find('ul'),
        tickerLi3 = tickerUl3.find('li'),
        liFirst = tickerUl3.find('li:first'),
        listWidth = tickerLi3.width();
    liFirst.css({ left: listWidth, display: 'block', opacity: '0', zIndex: '99' })
        .stop().animate({ left: '0', opacity: '1' }, 500, 'linear').addClass('showlist');

    setInterval(function () {
        var showLi = thisTicker3.find('.showlist');
        showLi.animate({ left: -(listWidth), opacity: '0' }, 500, 'linear')
            .next().css({ left: listWidth, display: 'block', opacity: '0', zIndex: '99' })
            .animate({ left: '0', opacity: '1' }, 500, 'linear').addClass('showlist')
            .end().appendTo(tickerUl3).css({ zIndex: '98' }).removeClass('showlist');
    }, 3500);

    $('body').append('<button id="fixedTop">▲</button>');
    $('#fixedTop').on('click', function () {
        console.log('hello');
        $('html,body').animate({ scrollTop: '0' }, 1000);
    });
    var setFilter = $('#filterBtn'),
        filterBtn = setFilter.find('button'),
        btnAll = $('.allItem'),
        setList = $('#filterList'),
        filterList = setList.find('li'),
        listWidth = filterList.outerWidth();
    filterBtn.click(function () {
        if (!($(this).hasClass('active'))) {
            var filterClass = $(this).attr('class');
            filterList.each(function () {
                if ($(this).hasClass(filterClass)) {
                    $(this).css({ display: 'block' }).stop().animate({ width: listWidth }, 1500);
                    $(this).find('*').stop().animate({ opacity: '1' }, 1500);
                } else {
                    $(this).find('*').stop().animate({ opacity: '0' }, 1000);
                    $(this).stop().animate({ width: '0' }, 1000, function () {
                        $(this).css({ display: 'none' });
                    });
                }
            });
            filterBtn.removeClass('active');
            $(this).addClass('active');
        }
    });
    btnAll.click(function () {
        filterList.each(function () {
            $(this).css({ display: 'block' }).stop().animate({ width: listWidth }, 1500);
            $(this).find('*').stop().animate({ opacity: '1' }, 1500);
        });
    });
    btnAll.click();


    setHeight2();
    $('#wrapper').css({ opacity: '1' });
    function setHeight1() {
        var setElm = $('.listArea ul li'),
            pdTop = parseInt(setElm.css('paddingTop')),
            pdBtm = parseInt(setElm.css('paddingBottom')),
            boxSizing = setElm.css('boxSizing'),
            h = 0;
        setElm.each(function () {
            var self = $(this).outerHeight();
            if (h < self) { h = self; }
        });
        if (boxSizing == 'border-box') {
            setElm.css({ height: h });
        } else {
            setElm.css({ hetght: h - (pdTop + pdBtm) });
        }
    }
    function setHeight2(){
        var liObj = $('.listArea ul li'),
            ulObj = $('.listArea ul'),
            hSize = 0,
            hMarg = parseInt(liObj.css('paddingTop')) + parseInt(liObj.css('paddingBottom')),
            boxSizing = liObj.css('boxSizing'),
            objLength = liObj.length - 1,
            numOfLine = Math.floor(ulObj.outerWidth() / liObj.outerWidth()),
            lineCnt = numOfLine - 1;
        if (boxSizing == 'border-box') { hMarg = 0; }

        liObj.each(function (i) {
            var self = $(this);
            self.addClass('replaceFlg');
            if (hSize < self.outerHeight()) { hSize = self.outerHeight(); }
            if (i % numOfLine == lineCnt || i == objLength) {
                ulObj.find('.replaceFlg').css({ height: hSize - hMarg }).removeClass('replaceFlg');
                hSize = 0;
            }
        });
    }
    var setSlideElm = $('.slider'),
        slideSpeed = 500,
        slideDelay = 5000,
        slideEasing = 'linear',
        openingFade = 1000;
    setSlideElm.each(function () {
        var self = $(this),
            findUl = self.find('ul'),
            findLi = findUl.find('li'),
            findLiCount = findLi.length,
            findImg = findLi.find('img'),
            sildeTimer;
        findLi.each(function (i) {
            $(this).attr('class', 'viewList' + (i + 1));
        });
        if (findLiCount > 1) {
            self.wrapAll('<div class="sliderCover" />');
            findUl.wrapAll('<div class = "sliderWrap" />');

            var findCover = self.parent('.sliderCover'),
                findWrap = self.find('.sliderWrap');

            var imgPrevL = findLi.slice(findLi.length - 2, findLi.length).clone().wrapAll('<ul class="dmy">').parent(),
                imgNextL = findLi.slice(0.2, findLi.length).clone().wrapAll('<ul class="dmy">').parent();
            imgPrevL.prependTo(findWrap);
            imgNextL.appendTo(findWrap);


            findWrap.find('ul').eq('1').addClass('mainList');
            var mainList = findWrap.find('.mainList').find('li');
            mainList.eq('0').addClass('mainActive');

            var allList = findWrap.find('li'),
                allListCount = allList.length;
            var imgWidth = findImg.width(),
                imgHeight = findImg.height();

            self.css({ height: imgHeight });
            findCover.css({ height: imgHeight });
            allList.css({ height: imgHeight });

            var baseWrapWidth = imgWidth * findLiCount,
                allWrapWidth = imgWidth * allListCount;
            findWrap.css({ left: -(imgWidth * 2), width: allWrapWidth, height: imgHeight });
            findWrap.find('ul.mainList').css({ width: baseWrapWidth, height: imgHeight });
            findWrap.find('ul.dmy').css({ width: imgWidth * 2, height: imgHeight });

            // ページネーションの追加
            var pagination = $('<div class="pagiNation"></div>');
            self.append(pagination);
            findLi.each(function (i) {
                pagination.append('<a href="#!" class="pn' + (i + 1) + '"></a>');
            })
            var pnPoint = pagination.find('a'),
                pnFirst = pagination.find('a:first'),
                pnLast = pagination.find('a:last'),
                pnCount = pnPoint.length;
            pnFirst.addClass('pnActive');

            pnPoint.click(function () {
                timerStop();
                var showCont = pnPoint.index(this) + 2,
                    //moveLeft = (imgWidth * showCont) + baseWrapWidth,
                    moveLeft = (imgWidth * showCont);
                findWrap.stop().animate({ left: -(moveLeft) }, slideSpeed, slideEasing);
                pnPoint.removeClass('pnActive');
                $(this).addClass('pnActive');
                activePos();
                timerStart();
            });
            function activePos() {
                var posActive = pagination.find('.pnActive'),
                    posIndex = pnPoint.index(posActive);
                findLi.removeClass('mainActive').eq(posIndex).addClass('mainActive');
            }
            // ナビボタンの追加
            self.append('<a href="#!" class="btnPrev"></a><a href="#!" class="btnNext"></a>');
            var btnNext = self.find('.btnNext'),
                btnPrev = self.find('.btnPrev'),
                posResetNext = -(baseWrapWidth + imgWidth),
                posResetPrev = -(imgWidth);
            function sideNavSize() {
                var slideWidth = self.width(),
                    btnSize = ($(window).width() - slideWidth) / 2;
                if ($(window).width() > slideWidth) {
                    btnNext.css({ right: -btnSize, width: btnSize, height: imgHeight });
                    btnPrev.css({ left: -btnSize, width: btnSize, height: imgHeight });

                }
            }
            sideNavSize();

            $(window).on('resize', function () {
                sideNavSize();
            });
                

            function slideNext() {
                findWrap.not(':animated').each(function () {
                    timerStop();
                    var posLeft = parseInt($(findWrap).css('left')),
                        moveLeft = posLeft - imgWidth;
                    //if (moveLeft < posResetNext) {
                    //    moveLeft = - (imgWidth);
                    //}
                    findWrap.stop().animate({ left: (moveLeft) },
                        slideSpeed, slideEasing, function () {
                            //var adjustLeft = parseInt($(findWrap).css('left'));
                            if (moveLeft < posResetNext) {
                                findWrap.css({ left: -(imgWidth * 2) });
                            }
                    });
                    var setActive = pagination.find('.pnActive'),
                        pnIndex = pnPoint.index(setActive),
                        listCount = pnIndex + 1;
                    if (pnCount == listCount) {
                        setActive.removeClass('pnActive');
                        pnFirst.addClass('pnActive');
                    } else {
                        setActive.removeClass('pnActive').next().addClass('pnActive');
                    }
                    activePos();
                    timerStart();
                });
            }

            function slidePrev() {
                findWrap.not(':animated').each(function () {
                    timerStop();
                    var posLeft = parseInt($(findWrap).css('left')),
                        moveLeft = posLeft + imgWidth;
                    findWrap.stop().animate({ left: (moveLeft) },
                        slideSpeed, slideEasing, function () {
                            if (moveLeft >= posResetPrev) {
                                findWrap.css({ left: posResetNext });
                            }
                        });
                    var setActive = pagination.find('.pnActive'),
                        pnIndex = pnPoint.index(setActive),
                        listCount = pnIndex + 1;
                    if (pnIndex == 0) {
                        setActive.removeClass('pnActive');
                        pnLast.addClass('pnActive');
                    } else {
                        setActive.removeClass('pnActive').prev().addClass('pnActive');
                    }
                    activePos();
                    timerStart();
                });
            }

            $('.btnNext').on('click', function () { slideNext(); });
            $('.btnPrev').on('click', function () { slidePrev(); });


            function timerStart() {
                slideTimer = setInterval(function () {
                    slideNext();
                }, slideDelay);
            }
            function timerStop() {
                clearInterval(slideTimer);
            }
            timerStart();
            self.css({ visibility: 'visible', opacity: '0' }).animate({ opacity: '1' }, openingFade);
        }
    });

});


//普通のジャバすくはここに書こう
