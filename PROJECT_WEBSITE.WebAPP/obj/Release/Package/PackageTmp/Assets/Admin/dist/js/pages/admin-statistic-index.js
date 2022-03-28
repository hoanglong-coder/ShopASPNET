var pg = {
    pageSize: 10,
    pageIndex: 1
}

var pgtheonhanvien = {
    pageSize: 10,
    pageIndex: 1
}

var pgtheokhachhang = {
    pageSize: 10,
    pageIndex: 1
}

var pgtheokhachhangdetail = {
    pageSize: 10,
    pageIndex: 1
}

var pgtheosanpham = {
    pageSize: 10,
    pageIndex: 1
}

var pgtheonhanviendetail = {
    pageSize: 10,
    pageIndex: 1
}

var pgtheosanphamdetail = {
    pageSize: 10,
    pageIndex: 1
}


var pgngay = {
    pageSize: 10,
    pageIndex: 1
}
//Thông báo
var Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000
});

//Main
var StatisticController = {
    init: function () {
        //Khởi tạo theme
        StatisticController.initTheme();

        StatisticController.showngaydetail();

        //Danh sách thống kê thắng
        StatisticController.lstthongketngay();

        //Search Theo ngày
        StatisticController.searchtheongay();

        //Xuất ds thống kê từ ngày đến ngày
        StatisticController.ExportDSOrderNgay();

        //Theo nhân viên
        //Danh sách thống kê theo nhân viên
        StatisticController.lstthongketnhanvien();

        //Searh theo nhân viên
        StatisticController.searchthongketnhanvien();

        StatisticController.shownhanviendetail();

        //Xuất ds thống kê của nhân viên
        StatisticController.ExportDSOrderNhanVien();

        //Theo khách hàng
        StatisticController.showkhachhangdetail();

        //Danh sách thống kê theo khách hàng
        StatisticController.lstthongketkhachhang();

        //Searach thống kê khách hàng
        StatisticController.searchthongketkhachhang();

        StatisticController.ExportDSOrderKhachHang();

        //Theo sản phẩm 
        StatisticController.lstthongketsanpham();

        StatisticController.showsanphamdetail();

        StatisticController.searchthongketsanpham();

        StatisticController.ExportDSOrderSanPham();


    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    listOrderDetail: function () {
        $('.modalshowsub').on('click', function () {

            var id = $(this).data('id');
            $("#OrderID").val('HD' + ChangeIDProduct(id));
            $("#OrderCreateDate").val($(this).data('createdate'));
            $("#OrderUserName").val($(this).data('username'));
            $("#OrderCustomerName").val($(this).data('customername'));
            $("#OrderPhone").val($(this).data('phone'));
            $("#OrderEmail").val($(this).data('email'));
            $("#OrderDiaChi").val($(this).data('address'));
            $("#OrderGhichu").val($(this).data('ghichu'))
            $.ajax({
                url: '/admin/order/ListOrderDetail',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        var html = '';
                        var template = $('#data-tbOrderDetail').html();
                        var dem = 1;
                        $.each(data.data, function (key, value) {
                            html += Mustache.render(template, {
                                STT: dem,
                                ID: 'SP'+ChangeIDProduct(value.Product.ProductID),
                                Name: value.Product.Name,
                                Price: new Intl.NumberFormat().format(value.Price),
                                Quantity: value.Quantity,
                                Totalprice: new Intl.NumberFormat().format(value.TotalPrice),
                                DVT: value.DVT
                            });
                            dem++;
                        });
                        $('#tbOrderDetail').html(html);


                        var quantitysub = 0;
                        var total = 0;
                        $.each(data.data, function (key, value) {
                            quantitysub += value.Quantity;
                            total += value.TotalPrice
                        })

                        $('#TongSoLuong').empty();
                        $('#TongSoLuong').append(quantitysub);

                        $('#KhuyenMai').empty();
                        $('#KhuyenMai').append('-' + new Intl.NumberFormat().format(data.khuyenmai));

                        $('#TongTienHang').empty();
                        $('#TongTienHang').append(new Intl.NumberFormat().format(total));



                        let newtotal = 0;
                        if (total < 300000) {
                            newtotal = (total - data.khuyenmai) + 25000;
                            $('#TienShip').empty();
                            $('#TienShip').append('25.000');
                        } else {
                            newtotal = (total - data.khuyenmai);
                            $('#TienShip').empty();
                            $('#TienShip').append('0');
                        }

                        $('#TongTienTatCa').empty();
                        $('#TongTienTatCa').append(new Intl.NumberFormat().format(newtotal));
                        $('#suborderdetail').modal('show')
                    }
                }
            });
        });
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

    /*Theo ngày*/
    showngaydetail: function () {
        $(".shownoidung").off('click').on('click', function () {
            $('.list').toggle();
            var date = $(this).data('date');

            if (date != null) {
                $("#DoanhSoNgay").empty();
                $("#DoanhSoNgay").append('DOANH SỐ NGÀY: ' + date)
                $('#paginationOrderNgay').twbsPagination('destroy');
                StatisticController.lstthongkengaydetail(date);
            }
        })
    },
    lstthongkengaydetail: function (date, changePageSize) {
        $.ajax({
            url: '/admin/Statistic/GetThongKeDetailTheoNgay',
            type: 'post',
            data: {
                page: pgngay.pageIndex,
                pageSize: pgngay.pageSize,
                createDate: date
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = '';
                    var template = $('#data-templateOrderNgay').html();
                    let totalprice = data.totalprice;
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.OrderID,
                            DisplayID: 'HD' + ChangeIDProduct(value.OrderID),
                            NameID: 'KH' + ChangeIDProduct(value.CustomerID),
                            Name: value.ShipName,
                            Phone: value.ShipPhone,
                            Address: value.ShipAddress,
                            Email: value.ShipEmail,
                            Totalprice: new Intl.NumberFormat().format(value.TotalPrice),
                            TotalCount: value.TotalCount,
                            CREATEDATE: parseJsonDate(value.CreateDate),
                            Email: value.ShipEmail,
                            UserName: value.UserName,
                            PaymentStatus: value.PaymentName,
                            st: value.OrderStatus,
                            Ghichu: value.Discription
                        });
                    });
                    $('#tbOrderNgay').html(html);

                    $("#CountProductNgay").empty();
                    $("#CountProductNgay").append(data.totalcout);
                    $("#TotalPriceOrderNgay").empty();
                    $("#TotalPriceOrderNgay").append(new Intl.NumberFormat().format(data.totalprice));

                    StatisticController.pagingNgay(data.total, function () {
                        StatisticController.lstthongkengaydetail(date);
                    }, changePageSize)

                    StatisticController.showngaydetail();
                    StatisticController.listOrderDetail();
                    StatisticController.ExportOrderNgay(date);
                }
            }
        });
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
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    lstthongketngay: function (changePageSize) {

        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeThang").val();
        search.DenNgay = $("#DenNgaThongKeThangy").val();

        $.ajax({
            url: '/admin/Statistic/GetThongKeTheoNgay',
            type: 'post',
            data: {
                page: pg.pageIndex,
                pageSize: pg.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var toltal = 0;
                    var template = $('#data-templatetheothang').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            Date: parseJsonDate(value.Ngay),
                            CountOrder: value.SoDonHang,
                            CountProduct: value.SoLuong,
                            Total: new Intl.NumberFormat().format(value.TienHang),
                        });
                        toltal += value.TienHang;
                    });
                    $('#tbtheothang').html(html);

                    $('#SoDonHang').empty();
                    $('#SoDonHang').append(data.countorder);

                    $('#Soluongsanpham').empty();
                    $('#Soluongsanpham').append(data.countproduct);

                    $('#TongTien').empty();
                    $('#TongTien').append(new Intl.NumberFormat().format(toltal));

                    if (data.total != 0) {
                        StatisticController.paging(data.total, function () {
                            StatisticController.lstthongketngay();
                        }, changePageSize)
                    }

                    StatisticController.showngaydetail();
                }
            }
        });

        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')}`
        }
    },
    searchtheongay: function () {
        $("#btnTiemKiemTheoNgay").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationtheothang').twbsPagination('destroy');
            StatisticController.lstthongketngay();
        })

    },   
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#paginationtheothang a').length === 0 || changePageSize === true) {
            $('#paginationtheothang').empty();
            $('#paginationtheothang').removeData('twbs-pagination');
            $('#paginationtheothang').unbind("page");
        }
        $('#paginationtheothang').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pg.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    pagingNgay: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgngay.pageSize)
        if ($('#paginationOrderNgay a').length === 0 || changePageSize === true) {
            $('#paginationOrderNgay').empty();
            $('#paginationOrderNgay').removeData('twbs-pagination');
            $('#paginationOrderNgay').unbind("page");
        }
        $('#paginationOrderNgay').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgngay.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    ExportOrderNgay: function (date) {
        $("#ExportExcelOrderNgay").off('click').on('click', function () {
            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelListThongkeNgay',
                type: 'post',
                data: {
                    createDate: date
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })

        })
        $("#ExportPDFOrderNgay").off('click').on('click', function () {
            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFListThongkeNgay',
                type: 'post',
                data: {
                    createDate: date
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },
    ExportDSOrderNgay: function () {
        $("#ExportExcelOrderDSNgay").off('click').on('click', function () {

            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeThang").val();
            search.DenNgay = $("#DenNgaThongKeThangy").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelThongkeNgay',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })
        })
        $("#ExportPDFOrderDSNgay").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeThang").val();
            search.DenNgay = $("#DenNgaThongKeThangy").val();
            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFThongkeNgay',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },

    /*Theo nhân viên*/
    lstthongketnhanvien: function (changePageSize) {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeNhanvien").val();
        search.DenNgay = $("#DenNgaThongKeNhanvien").val();

        $.ajax({
            url: '/admin/Statistic/GetThongKeTheoNhanVien',
            type: 'post',
            data: {
                page: pgtheonhanvien.pageIndex,
                pageSize: pgtheonhanvien.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var toltal = 0;
                    var template = $('#data-templatetheoNhanvien').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            UserName: value.UserName,
                            UserID: value.UserID,
                            CountOrder: value.SoDonHang,
                            CountProduct: value.SoLuong,
                            Total: new Intl.NumberFormat().format(value.TienHang),
                        });
                        toltal += value.TienHang;
                    });
                    $('#tbtheoNhanvien').html(html);

                    $('#SoDonHangTheonhanvien').empty();
                    $('#SoDonHangTheonhanvien').append(data.countorder);

                    $('#SoluongsanphamTheonhanvien').empty();
                    $('#SoluongsanphamTheonhanvien').append(data.countproduct);

                    $('#TongTienTheonhanvien').empty();
                    $('#TongTienTheonhanvien').append(new Intl.NumberFormat().format(toltal));

                    if (data.total != 0) {
                        StatisticController.pagingtheonhanvien(data.total, function () {
                            StatisticController.lstthongketnhanvien();
                        }, changePageSize)
                    }
                    StatisticController.shownhanviendetail();
                }
            }
        });
    },
    searchthongketnhanvien: function () {
        $("#btnTiemKiemTheoNhanvien").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationtheoNhanvien').twbsPagination('destroy');
            StatisticController.lstthongketnhanvien();
        })
    },
    shownhanviendetail: function () {
        $(".shownoidungnhanvien").off('click').on('click', function () {
            $('.list2').toggle();
            var id = $(this).data('id');

            var name = $(this).data('name');
            if (id != null) {
                $("#DoanhSoNhanvien").empty();
                $("#DoanhSoNhanvien").append('DOANH SỐ NHÂN VIÊN: ' + name)
                $('#paginationOrderNhanvienDetail').twbsPagination('destroy');
                StatisticController.lstthongkenhanviendetail(id);
            }
        })
    },
    lstthongkenhanviendetail: function (id, changePageSize) {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeNhanvien").val();
        search.DenNgay = $("#DenNgaThongKeNhanvien").val();

        $.ajax({
            url: '/admin/Statistic/GetThongKeDetailTheoNhanvien',
            type: 'post',
            data: {
                page: pgtheonhanviendetail.pageIndex,
                pageSize: pgtheonhanviendetail.pageSize,
                id: id,
                search:search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = '';
                    var template = $('#data-templateOrderNgay').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.OrderID,
                            DisplayID: 'HD' + ChangeIDProduct(value.OrderID),
                            NameID: 'KH' + ChangeIDProduct(value.CustomerID),
                            Name: value.ShipName,
                            Phone: value.ShipPhone,
                            Address: value.ShipAddress,
                            Email: value.ShipEmail,
                            Totalprice: new Intl.NumberFormat().format(value.TotalPrice),
                            TotalCount: value.TotalCount,
                            CREATEDATE: parseJsonDate(value.CreateDate),
                            Email: value.ShipEmail,
                            UserName: value.UserName,
                            PaymentStatus: value.PaymentName,
                            st: value.OrderStatus,
                            Ghichu: value.Discription
                        });
                    });
                    $('#tbOrdernhanviendetail').html(html);

                    $("#CountProductNhanvien").empty();
                    $("#CountProductNhanvien").append(data.totalcout);
                    $("#TotalPriceOrderNhanVien").empty();
                    $("#TotalPriceOrderNhanVien").append(new Intl.NumberFormat().format(data.totalprice));

                    StatisticController.pagingtheonhanviendetail(data.total, function () {
                        StatisticController.lstthongkenhanviendetail(id);
                    }, changePageSize)

                    StatisticController.shownhanviendetail();
                    StatisticController.listOrderDetail();
                    StatisticController.ExportOrderNhanVien(id);
                }
            }
        });
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
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    pagingtheonhanvien: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgtheonhanvien.pageSize)
        if ($('#paginationtheoNhanvien a').length === 0 || changePageSize === true) {
            $('#paginationtheoNhanvien').empty();
            $('#paginationtheoNhanvien').removeData('twbs-pagination');
            $('#paginationtheoNhanvien').unbind("page");
        }
        $('#paginationtheoNhanvien').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgtheonhanvien.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    pagingtheonhanviendetail: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgtheonhanviendetail.pageSize)
        if ($('#paginationOrderNhanvienDetail a').length === 0 || changePageSize === true) {
            $('#paginationOrderNhanvienDetail').empty();
            $('#paginationOrderNhanvienDetail').removeData('twbs-pagination');
            $('#paginationOrderNhanvienDetail').unbind("page");
        }
        $('#paginationOrderNhanvienDetail').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgtheonhanviendetail.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    ExportOrderNhanVien: function (id) {
        $("#ExportExcelOrderNhanVien").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeNhanvien").val();
            search.DenNgay = $("#DenNgaThongKeNhanvien").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelListThongkeNhanVien',
                type: 'post',
                data: {
                    search: search,
                    id:id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })

        })
        $("#ExportPDFOrderNhanVien").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeNhanvien").val();
            search.DenNgay = $("#DenNgaThongKeNhanvien").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFListThongkeNhanVien',
                type: 'post',
                data: {
                    search: search,
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },
    ExportDSOrderNhanVien: function () {
        $("#ExportExcelOrderDSNhanvien").off('click').on('click', function () {

            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeNhanvien").val();
            search.DenNgay = $("#DenNgaThongKeNhanvien").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelThongkeNhanVien',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })
        })
        $("#ExportPDFOrderDSNhanvien").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeNhanvien").val();
            search.DenNgay = $("#DenNgaThongKeNhanvien").val();
            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFThongkeNhanVien',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },

    /*Theo khách hàng*/
    showkhachhangdetail: function () {
        $(".shownoidungkhachhang").off('click').on('click', function () {
            $('.list3').toggle();
            var id = $(this).data('id');

            if (id != null) {
                $("#DoanhSoKhachHang").empty();
                $("#DoanhSoKhachHang").append('DOANH SỐ KHÁCH HÀNG: ' + 'KH'+ChangeIDProduct(id))
                $('#paginationOrderNgay').twbsPagination('destroy');
                StatisticController.lstthongkekhachhangdetail(id);
            }
        })
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
    searchthongketkhachhang: function () {
        $("#btnTiemKiemTheoKhachhang").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationtheoKhachhang').twbsPagination('destroy');
            StatisticController.lstthongketkhachhang();
        })
    },
    lstthongketkhachhang: function (changePageSize) {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeKhachhang").val();
        search.DenNgay = $("#DenNgaThongKeKhachhang").val();

        $.ajax({
            url: '/admin/Statistic/GetThongKeTheoKhachHang',
            type: 'post',
            data: {
                page: pgtheokhachhang.pageIndex,
                pageSize: pgtheokhachhang.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var toltal = 0;
                    var template = $('#data-templatetheoKhachhang').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            CustomerName: value.CustomerName,
                            CustomerID: value.CustomerID,
                            CustomerIDDisplay: 'KH' + ChangeIDProduct(value.CustomerID),
                            CountOrder: value.SoDonHang,
                            CountProduct: value.SoLuong,
                            Total: new Intl.NumberFormat().format(value.TienHang),
                        });
                        toltal += value.TienHang;
                    });
                    $('#tbtheoKhachhang').html(html);

                    $('#SoDonHangTheokhachhang').empty();
                    $('#SoDonHangTheokhachhang').append(data.countorder);

                    $('#SoluongsanphamTheokhachhang').empty();
                    $('#SoluongsanphamTheokhachhang').append(data.countproduct);

                    $('#TongTienTheokhachhang').empty();
                    $('#TongTienTheokhachhang').append(new Intl.NumberFormat().format(toltal));

                    if (data.total != 0) {
                        StatisticController.pagingtheokhachhang(data.total, function () {
                            StatisticController.lstthongketkhachhang();
                        }, changePageSize)
                    }
                    StatisticController.showkhachhangdetail();
                }
            }
        });
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
    lstthongkekhachhangdetail: function (id, changePageSize) {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeKhachhang").val();
        search.DenNgay = $("#DenNgaThongKeKhachhang").val();

        $.ajax({
            url: '/admin/Statistic/GetThongKeDetailTheoKhachHang',
            type: 'post',
            data: {
                page: pgtheokhachhangdetail.pageIndex,
                pageSize: pgtheokhachhangdetail.pageSize,
                id: id,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = '';
                    var template = $('#data-templateOrderKhachhangDetail').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.OrderID,
                            DisplayID: 'HD' + ChangeIDProduct(value.OrderID),
                            NameID: 'KH' + ChangeIDProduct(value.CustomerID),
                            Name: value.ShipName,
                            Phone: value.ShipPhone,
                            Address: value.ShipAddress,
                            Email: value.ShipEmail,
                            Totalprice: new Intl.NumberFormat().format(value.TotalPrice),
                            TotalCount: value.TotalCount,
                            CREATEDATE: parseJsonDate(value.CreateDate),
                            Email: value.ShipEmail,
                            UserName: value.UserName,
                            PaymentStatus: value.PaymentName,
                            st: value.OrderStatus,
                            Ghichu: value.Discription
                        });
                    });
                    $('#tbOrderkhachhangdetail').html(html);

                    $("#CountProductKhachHang").empty();
                    $("#CountProductKhachHang").append(data.totalcout);
                    $("#TotalPriceOrderKhachHang").empty();
                    $("#TotalPriceOrderKhachHang").append(new Intl.NumberFormat().format(data.totalprice));

                    StatisticController.pagingtheokhachhangdetail(data.total, function () {
                        StatisticController.lstthongkekhachhangdetail(id);
                    }, changePageSize)

                    StatisticController.showkhachhangdetail();
                    StatisticController.listOrderDetail();
                    StatisticController.ExportOrderKhachHang(id);
                }
            }
        });
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
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    ExportOrderKhachHang: function (id) {
        $("#ExportExcelOrderKhachHang").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeKhachhang").val();
            search.DenNgay = $("#DenNgaThongKeKhachhang").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelListThongkeKhachhang',
                type: 'post',
                data: {
                    search: search,
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })

        })
        $("#ExportPDFOrderKhachHang").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeKhachhang").val();
            search.DenNgay = $("#DenNgaThongKeKhachhang").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFListThongkeKhachhang',
                type: 'post',
                data: {
                    search: search,
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },
    ExportDSOrderKhachHang: function () {
        $("#ExportExcelOrderDSKhachhang").off('click').on('click', function () {

            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeKhachhang").val();
            search.DenNgay = $("#DenNgaThongKeKhachhang").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelThongkeKhachhang',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })
        })
        $("#ExportPDFOrderDSKhachhang").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeKhachhang").val();
            search.DenNgay = $("#DenNgaThongKeKhachhang").val();
            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFThongkeKhachhang',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },
    pagingtheokhachhang: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgtheokhachhang.pageSize)
        if ($('#paginationtheoKhachhang a').length === 0 || changePageSize === true) {
            $('#paginationtheoKhachhang').empty();
            $('#paginationtheoKhachhang').removeData('twbs-pagination');
            $('#paginationtheoKhachhang').unbind("page");
        }
        $('#paginationtheoKhachhang').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgtheokhachhang.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    pagingtheokhachhangdetail: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgtheokhachhangdetail.pageSize)
        if ($('#paginationOrderkhachhangDetail a').length === 0 || changePageSize === true) {
            $('#paginationOrderkhachhangDetail').empty();
            $('#paginationOrderkhachhangDetail').removeData('twbs-pagination');
            $('#paginationOrderkhachhangDetail').unbind("page");
        }
        $('#paginationOrderkhachhangDetail').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgtheokhachhangdetail.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    

    /*Theo sản phẩm*/
    lstthongketsanpham: function (changePageSize) {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeSanpham").val();
        search.DenNgay = $("#DenNgaThongKeSanpham").val();

        $.ajax({
            url: '/admin/Statistic/GetThongKeTheoSanPham',
            type: 'post',
            data: {
                page: pgtheosanpham.pageIndex,
                pageSize: pgtheosanpham.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    
                    var html = "";
                    var toltal = 0;
                    var template = $('#data-templatetheoSanpham').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ProductID: value.ProductID,
                            ProductName: value.ProductName,
                            ProductIDDisplay: value.ProductIDDisplay,
                            CountOrder: value.SoDonHang,
                            DVT: value.DVT,
                            CountProduct: value.SoLuong,
                            Total: new Intl.NumberFormat().format(value.TienHang),
                        });
                        toltal += value.TienHang;
                    });
                    $('#tbtheoSanpham').html(html);

                    $('#SoDonHangTheoSanpham').empty();
                    $('#SoDonHangTheoSanpham').append(data.countorder);

                    $('#SoluongsanphamTheoSanpham').empty();
                    $('#SoluongsanphamTheoSanpham').append(data.countproduct);

                    $('#TongTienTheoSanpham').empty();
                    $('#TongTienTheoSanpham').append(new Intl.NumberFormat().format(toltal));

                    if (data.total != 0) {
                        StatisticController.pagingtheosanpham(data.total, function () {
                            StatisticController.lstthongketsanpham();
                        }, changePageSize)
                    }
                    StatisticController.showsanphamdetail();
                }
            }
        });
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
    showsanphamdetail: function () {
        $(".shownoidungsanpham").off('click').on('click', function () {
            $('.list4').toggle();
            var name = $(this).data('name');
            var id = $(this).data('id');

            if (id != null) {
                $("#DoanhSoSanPham").empty();
                $("#DoanhSoSanPham").append('DOANH SỐ SẢN PHẨM: ' + name)
                $('#paginationOrderNgay').twbsPagination('destroy');
                StatisticController.lstthongkesanphamdetail(id);
            }
        })     
    },
    lstthongkesanphamdetail: function (id, changePageSize) {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeSanpham").val();
        search.DenNgay = $("#DenNgaThongKeSanpham").val();

        $.ajax({
            url: '/admin/Statistic/GetThongKeDetailTheoSanPham',
            type: 'post',
            data: {
                page: pgtheosanphamdetail.pageIndex,
                pageSize: pgtheosanphamdetail.pageSize,
                id: id,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    console.log(data.data)
                    var html = '';
                    var template = $('#data-templateOrdersanphamDetail').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.OrderID,
                            DisplayID: 'HD' + ChangeIDProduct(value.OrderID),
                            ProductIDDisplay: value.ProductIDDisplay,
                            DVT: value.DVT,
                            Name: value.ProductName,
                            Phone: value.ShipPhone,
                            Address: value.ShipAddress,
                            Email: value.ShipEmail,
                            Totalprice: new Intl.NumberFormat().format(value.ThanhTien),
                            TotalCount: value.SoLuong,
                            Price: new Intl.NumberFormat().format(value.DonGia),
                            CREATEDATE: parseJsonDate(value.CreateDate),
                            Email: value.ShipEmail,
                            UserName: value.NguoiBan,
                            st: value.OrderStatus,
                            Ghichu: value.Discription
                        });
                    });
                    $('#tbOrdersanphamdetail').html(html);

                    $("#CountProductSanPham").empty();
                    $("#CountProductSanPham").append(data.totalcout);
                    $("#TotalPriceOrderSanPham").empty();
                    $("#TotalPriceOrderSanPham").append(new Intl.NumberFormat().format(data.totalprice));

                    StatisticController.pagingtheosanphamdetail(data.total, function () {
                        StatisticController.lstthongkesanphamdetail(id);
                    }, changePageSize)

                    StatisticController.showsanphamdetail();
                    StatisticController.listOrderDetail();
                    StatisticController.ExportOrderSanPham(id);
                }
            }
        });
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
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    searchthongketsanpham: function () {
        $("#btnTiemKiemTheoSanpham").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationtheoSanpham').twbsPagination('destroy');
            StatisticController.lstthongketsanpham();
        })
    },
    pagingtheosanpham: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgtheosanpham.pageSize)
        if ($('#paginationtheoSanpham a').length === 0 || changePageSize === true) {
            $('#paginationtheoSanpham').empty();
            $('#paginationtheoSanpham').removeData('twbs-pagination');
            $('#paginationtheoSanpham').unbind("page");
        }
        $('#paginationtheoSanpham').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgtheosanpham.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    pagingtheosanphamdetail: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgtheosanphamdetail.pageSize)
        if ($('#paginationOrdersanphamDetail a').length === 0 || changePageSize === true) {
            $('#paginationOrdersanphamDetail').empty();
            $('#paginationOrdersanphamDetail').removeData('twbs-pagination');
            $('#paginationOrdersanphamDetail').unbind("page");
        }
        $('#paginationOrdersanphamDetail').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgtheosanphamdetail.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    ExportOrderSanPham: function (id) {
        $("#ExportExcelOrderSanPham").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeSanpham").val();
            search.DenNgay = $("#DenNgaThongKeSanpham").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelListThongkeSanPham',
                type: 'post',
                data: {
                    search: search,
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })

        })
        $("#ExportPDFOrderSanPham").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeSanpham").val();
            search.DenNgay = $("#DenNgaThongKeSanpham").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFListThongkeSanPham',
                type: 'post',
                data: {
                    search: search,
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },
    ExportDSOrderSanPham: function () {
        $("#ExportExcelOrderDSSanpham").off('click').on('click', function () {

            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeSanpham").val();
            search.DenNgay = $("#DenNgaThongKeSanpham").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportExcelThongkeSanPham',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            location.href = "/Data/ExportExcel/" + data.NameFile;
                        }
                    }
                }

            })
        })
        $("#ExportPDFOrderDSSanpham").off('click').on('click', function () {
            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeSanpham").val();
            search.DenNgay = $("#DenNgaThongKeSanpham").val();
            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/Statistic/ExportPDFThongkeSanPham',
                type: 'post',
                data: {
                    search: search
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {

                            location.href = "/Data/ExportPDF/" + data.NameFile;

                        }
                    }
                }

            })
        })
    },
}
StatisticController.init();