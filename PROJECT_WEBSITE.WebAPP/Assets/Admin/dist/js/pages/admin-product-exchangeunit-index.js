var pgExchangeUnit = {
    pageSize: 3,
    pageIndex: 1
}


//Main
var ExchangeUnitController = {
    init: function () {
        //Khởi tạo theme
        ExchangeUnitController.initTheme();

        //Danh sách Phiếu quy đổi
        ExchangeUnitController.lstExchangeUnit();

        //Search Phiếu quy đổi
        ExchangeUnitController.searchExchangeUnit();
    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    lstExchangeUnit: function (changePageSize) {
        var search = new Object();
        search.SearchName = $("#search-queryexchangeunit").val();
        search.TuNgay = $("#TuNgayexchangeunit").val();
        search.DenNgay = $("#DenNgayexchangeunit").val();

        $.ajax({
            url: '/admin/exchangeunit/GetAll',
            type: 'post',
            data: {
                page: pgExchangeUnit.pageIndex,
                pageSize: pgExchangeUnit.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templateExchangeUnit').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ExchangeUnitIDDisplay: 'QD' + ChangeIDProduct(value.ExchangeUnitID),
                            Createdate: parseJsonDate(value.Createdate),
                            ProductNameIn: value.ProductNameIn,
                            ProductNameOut: value.ProductNameOut,
                            Result: value.Result,
                            UserName: value.UserName
                        });
                    });

                    $('#tbExchangeUnit').html(html);
                    if (data.total != 0) {
                        ExchangeUnitController.paging(data.total, function () {
                            ExchangeUnitController.lstExchangeUnit();
                        }, changePageSize)
                    }
                }
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }

    },
    searchExchangeUnit: function () {
        $("#btnTiemKiemexchangeunit").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationExchangeUnit').twbsPagination('destroy');
            ExchangeUnitController.lstExchangeUnit();
        })

    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgExchangeUnit.pageSize)
        if ($('#paginationExchangeUnit a').length === 0 || changePageSize === true) {
            $('#paginationExchangeUnit').empty();
            $('#paginationExchangeUnit').removeData('twbs-pagination');
            $('#paginationExchangeUnit').unbind("page");
        }
        $('#paginationExchangeUnit').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgExchangeUnit.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },

}
ExchangeUnitController.init();