var pgTheoThang = {
    pageSize: 10,
    pageIndex: 1
}

var pgTheoThangDetail = {
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

var pgtheosanphamdetail = {
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


var StatisticLNController = {
    init: function () {
        //Khởi tạo theme
        StatisticLNController.initTheme();

        //List 1 
        StatisticLNController.lstthongketngay();

        StatisticLNController.showngaydetail();

        StatisticLNController.ExportDSOrderNgay();

        StatisticLNController.searchtheongay();

        //List 2 
        StatisticLNController.lstthongketkhachhang();

        StatisticLNController.showkhachhangdetail();

        StatisticLNController.searchthongketkhachhang();

        StatisticLNController.ExportDSOrderKhachHang();


        StatisticLNController.baocao();

        StatisticLNController.searchthongketbaocao();


        StatisticLNController.ExportBaoCao();

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
                                ID: 'SP' + ChangeIDProduct(value.Product.ProductID),
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

    //Theo ngày
    showngaydetail: function () {
        $(".shownoidung").off('click').on('click', function () {
            $('.list').toggle();
            var date = $(this).data('date');

            if (date != null) {
                $("#DoanhSoNgay").empty();
                $("#DoanhSoNgay").append('LỢI NHUẬN NGÀY: ' + date)
                $('#paginationOrderNgay').twbsPagination('destroy');
                StatisticLNController.lstthongkengaydetail(date);
            }
        })
    },
    searchtheongay: function () {
        $("#btnTiemKiemTheoNgay").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationtheothang').twbsPagination('destroy');
            StatisticLNController.lstthongketngay();
        })

    },
    lstthongkengaydetail: function (date, changePageSize) {
        $.ajax({
            url: '/admin/StatisticLN/GetThongKeDetailTheoNgay',
            type: 'post',
            data: {
                page: pgTheoThangDetail.pageIndex,
                pageSize: pgTheoThangDetail.pageSize,
                createDate: date
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = '';
                    var template = $('#data-templateOrderNgay').html();
                    let totalprice = data.totalprice;
                    console.log(data.data)
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.OrderID,
                            DisplayID: 'HD' + ChangeIDProduct(value.OrderID),
                            Name: value.CustomerName,
                            Phone: value.ShipPhone,
                            Address: value.ShipAddress,
                            Email: value.ShipEmail,
                            TienHang: new Intl.NumberFormat().format(value.TienHang),
                            DoanhThu: new Intl.NumberFormat().format(value.DoanhThu),
                            Von: new Intl.NumberFormat().format(value.Von),
                            LaiGop: new Intl.NumberFormat().format(value.Lai),
                            CREATEDATE: parseJsonDate(value.CreateDate),
                            Email: value.ShipEmail,
                            UserName: value.UserName,
                            Ghichu: value.Discription
                        });
                    });
                    $('#tbOrderNgay').html(html);

                    $("#TienHangOrderNgay").empty();
                    $("#TienHangOrderNgay").append(new Intl.NumberFormat().format(data.tienhang));

                    $("#DoanhThuOrderNgay").empty();
                    $("#DoanhThuOrderNgay").append(new Intl.NumberFormat().format(data.doanthu));

                    $("#VonOrderNgay").empty();
                    $("#VonOrderNgay").append(new Intl.NumberFormat().format(data.von));

                    $("#LaiOrderNgay").empty();
                    $("#LaiOrderNgay").append(new Intl.NumberFormat().format(data.lai));

                    StatisticLNController.pagingtheongaydetail(data.total, function () {
                        StatisticLNController.lstthongkengaydetail(date);
                    }, changePageSize)

                    StatisticLNController.showngaydetail();
                    StatisticLNController.listOrderDetail();
                    StatisticLNController.ExportOrderNgay(date);
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
            url: '/admin/StatisticLN/GetThongKeTheoNgay',
            type: 'post',
            data: {
                page: pgTheoThang.pageIndex,
                pageSize: pgTheoThang.pageSize,
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
                            Date: parseJsonDate(value.CreateDate),
                            TienHang: new Intl.NumberFormat().format(value.TienHang),
                            DoanhThu: new Intl.NumberFormat().format(value.DoanhThu),
                            Von: new Intl.NumberFormat().format(value.Von),
                            LaiGop: new Intl.NumberFormat().format(value.Lai),
                        });
                        toltal += value.TienHang;
                    });
                    $('#tbtheothang').html(html);

                    $('#TienHang').empty();
                    $('#TienHang').append(new Intl.NumberFormat().format(data.tienhang));

                    $('#DoanhThu').empty();
                    $('#DoanhThu').append(new Intl.NumberFormat().format(data.doanthu));

                    $('#Von').empty();
                    $('#Von').append(new Intl.NumberFormat().format(data.von));

                    $('#Laigop').empty();
                    $('#Laigop').append(new Intl.NumberFormat().format(data.lai));

                    if (data.total != 0) {
                        StatisticLNController.pagingtheongay(data.total, function () {
                            StatisticLNController.lstthongketngay();
                        }, changePageSize)
                    }

                    StatisticLNController.showngaydetail();
                }
            }
        });

        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')}`
        }
    },
    pagingtheongay: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgTheoThang.pageSize)
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
                pgTheoThang.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    pagingtheongaydetail: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgTheoThangDetail.pageSize)
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
                pgTheoThangDetail.pageIndex = page;
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
                url: '/admin/StatisticLN/ExportExcelListLoiNhuanNgay',
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
                url: '/admin/StatisticLN/ExportPDFListLoiNhuanNgay',
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
                url: '/admin/StatisticLN/ExportExcelThongkeNgay',
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
                url: '/admin/StatisticLN/ExportPDFThongkeNgay',
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

    //Theo khách hàng
    showkhachhangdetail: function () {
        $(".shownoidungkhachhang").off('click').on('click', function () {
            $('.list3').toggle();
            var id = $(this).data('id');

            if (id != null) {
                $("#DoanhSoKhachHang").empty();
                $("#DoanhSoKhachHang").append('LỢI NHUẬN KHÁCH HÀNG: ' + 'KH' + ChangeIDProduct(id))
                $('#paginationOrderNgay').twbsPagination('destroy');
                StatisticLNController.lstthongkekhachhangdetail(id);
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
            StatisticLNController.lstthongketkhachhang();
        })
    },
    lstthongketkhachhang: function (changePageSize) {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeKhachhang").val();
        search.DenNgay = $("#DenNgaThongKeKhachhang").val();

        $.ajax({
            url: '/admin/StatisticLN/GetThongKeTheoKhachHang',
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
                            TienHang: new Intl.NumberFormat().format(value.TienHang),
                            DoanhThu: new Intl.NumberFormat().format(value.DoanhThu),
                            Von: new Intl.NumberFormat().format(value.Von),
                            Lai: new Intl.NumberFormat().format(value.Lai)
                        });
                    });
                    $('#tbtheoKhachhang').html(html);

                    $('#TienHangTheokhachhang').empty();
                    $('#TienHangTheokhachhang').append(new Intl.NumberFormat().format(data.tienhang));

                    $('#DoanhThuTheokhachhang').empty();
                    $('#DoanhThuTheokhachhang').append(new Intl.NumberFormat().format(data.doanthu));

                    $('#VonTheokhachhang').empty();
                    $('#VonTheokhachhang').append(new Intl.NumberFormat().format(data.von));

                    $('#LaiTheokhachhang').empty();
                    $('#LaiTheokhachhang').append(new Intl.NumberFormat().format(data.lai));

                    if (data.total != 0) {
                        StatisticLNController.pagingtheokhachhang(data.total, function () {
                            StatisticLNController.lstthongketkhachhang();
                        }, changePageSize)
                    }
                    StatisticLNController.showkhachhangdetail();
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
            url: '/admin/StatisticLN/GetThongKeDetailTheoKhachHang',
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
                            Name: value.CustomerName,
                            Phone: value.ShipPhone,
                            Address: value.ShipAddress,
                            Email: value.ShipEmail,
                            CREATEDATE: parseJsonDate(value.CreateDate),
                            Email: value.ShipEmail,
                            UserName: value.UserName,
                            st: value.OrderStatus,
                            Ghichu: value.Discription,
                            Tienhang: new Intl.NumberFormat().format(value.TienHang),
                            Doanhthu: new Intl.NumberFormat().format(value.DoanhThu),
                            Von: new Intl.NumberFormat().format(value.Von),
                            Laigop: new Intl.NumberFormat().format(value.Lai)
                        });
                    });
                    $('#tbOrderkhachhangdetail').html(html);

                    $("#TienHangKhachHang").empty();
                    $("#TienHangKhachHang").append(new Intl.NumberFormat().format(data.tienhang));

                    $("#DoanhThuKhachHang").empty();
                    $("#DoanhThuKhachHang").append(new Intl.NumberFormat().format(data.doanthu));

                    $("#VonKhachHang").empty();
                    $("#VonKhachHang").append(new Intl.NumberFormat().format(data.von));

                    $("#LaiKhachHang").empty();
                    $("#LaiKhachHang").append(new Intl.NumberFormat().format(data.lai));

                    StatisticLNController.pagingtheokhachhangdetail(data.total, function () {
                        StatisticLNController.lstthongkekhachhangdetail(id);
                    }, changePageSize)

                    StatisticLNController.showkhachhangdetail();
                    StatisticLNController.listOrderDetail();
                    StatisticLNController.ExportOrderKhachHang(id);
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
                url: '/admin/StatisticLN/ExportExcelListThongkeKhachhang',
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
                url: '/admin/StatisticLN/ExportPDFListThongkeKhachhang',
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
                url: '/admin/StatisticLN/ExportExcelThongkeKhachhang',
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
                url: '/admin/StatisticLN/ExportPDFThongkeKhachhang',
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
            url: '/admin/StatisticLN/GetThongKeTheoSanPham',
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
                            DVT: value.DVT,
                            CountProduct: value.SoLuong,
                            TienHang: new Intl.NumberFormat().format(value.TienHang),
                            DoanhThu: new Intl.NumberFormat().format(value.DoanhThu),
                            Von: new Intl.NumberFormat().format(value.Von),
                            Lai: new Intl.NumberFormat().format(value.Lai),
                        });
                        toltal += value.TienHang;
                    });
                    $('#tbtheoSanpham').html(html);

                    $('#SoluongsanphamTheoSanpham').empty();
                    $('#SoluongsanphamTheoSanpham').append(data.countproduct);

                    $('#TienHangTheoSanpham').empty();
                    $('#TienHangTheoSanpham').append(new Intl.NumberFormat().format(data.tienhang));

                    $('#DoanhThuTheoSanpham').empty();
                    $('#DoanhThuTheoSanpham').append(new Intl.NumberFormat().format(data.doanthu));

                    $('#VonTheoSanpham').empty();
                    $('#VonTheoSanpham').append(new Intl.NumberFormat().format(data.von));

                    $('#LaiTheoSanpham').empty();
                    $('#LaiTheoSanpham').append(new Intl.NumberFormat().format(data.lai));

                    

                    if (data.total != 0) {
                        StatisticLNController.pagingtheosanpham(data.total, function () {
                            StatisticLNController.lstthongketsanpham();
                        }, changePageSize)
                    }
                    StatisticLNController.showsanphamdetail();
                }
            }
        });
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
                StatisticLNController.lstthongkesanphamdetail(id);
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

                    StatisticLNController.pagingtheosanphamdetail(data.total, function () {
                        StatisticLNController.lstthongkesanphamdetail(id);
                    }, changePageSize)

                    StatisticLNController.showsanphamdetail();
                    StatisticLNController.listOrderDetail();
                    StatisticLNController.ExportOrderSanPham(id);
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
            StatisticLNController.lstthongketsanpham();
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


    //Báo cáo lãi lổ
    baocao: function () {
        var search = new Object();
        search.TuNgay = $("#TuNgayThongKeBaoCao").val();
        search.DenNgay = $("#DenNgayThongKeBaoCao").val();
        $.ajax({
            url: '/admin/StatisticLN/BaoCaoLaiLo',
            type: 'post',
            data: {
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    $("#KyTruocBaoCao").empty();
                    $("#KyTruocBaoCao").append('Kỳ trước (' + parseJsonDate(data.data.KyTruocTu) + '-' + parseJsonDate(data.data.KyTruocDen)+')');

                    $("#KySauBaoCao").empty();
                    $("#KySauBaoCao").append('Kỳ Báo cáo (' + parseJsonDate(data.data.KyBaoCaoTu) + '-' + parseJsonDate(data.data.KyBaoCaoDen)+')');

                    var html = "";
                    var template = $('#data-templatebaocao').html();
                    html += Mustache.render(template, {
                        DoanhSoBanHang: '1.'+ data.data.DoanhSoBanHang,
                        KyTruocDoanhSoBanHang: Non(data.data.KyTruocDoanhSoBanHang),
                        KyBaoCaoDoanhSoBanHang: Non(data.data.KyBaoCaoDoanhSoBanHang),
                        ThaydoiDoanhSoBanHang: Color(data.data.ThaydoiDoanhSoBanHang),
                        GiamGia: '2.' + data.data.GiamGia,
                        KyTruocGiamGia: Non(data.data.KyTruocGiamGia),
                        KyBaoCaoGiamGia: Non(data.data.KyBaoCaoGiamGia),
                        ThaydoiGiamGia: Color(data.data.ThaydoiGiamGia),
                        PhiVanChuyen: '3.' +data.data.PhiVanChuyen,
                        KyTruocPhiVanChuyen: Non(data.data.KyTruocPhiVanChuyen),
                        KyBaoCaoPhiVanChuyen: Non(data.data.KyBaoCaoPhiVanChuyen),
                        ThaydoiPhiVanChuyen: Color(data.data.ThaydoiPhiVanChuyen),
                        DoanhThu: '4.' + data.data.DoanhThu,
                        KyTruocDoanhThu: Non(data.data.KyTruocDoanhThu),
                        KyBaoCaoDoanhThu: Non(data.data.KyBaoCaoDoanhThu),
                        ThaydoiDoanhThu: Color(data.data.ThaydoiDoanhThu),
                        VonHangHoa: '5.' + data.data.VonHangHoa,
                        KyTruocVonHangHoa: Non(data.data.KyTruocVonHangHoa),
                        KyBaoCaoVonHangHoa: Non(data.data.KyBaoCaoVonHangHoa),
                        ThaydoiVonHangHoa: Color(data.data.ThaydoiVonHangHoa),
                        LaiGop: '6.' + data.data.LaiGop,
                        KyTruocLaiGop: Non(data.data.KyTruocLaiGop),
                        KyBaoCaoLaiGop: Non(data.data.KyBaoCaoLaiGop),
                        ThaydoiLaigop: Color(data.data.ThaydoiLaigop),
                        TienGiamChiaDoanhThu: '7.' + data.data.TienGiamChiaDoanhThu,
                        KyTruocTienGiamChiaDoanhThu: NonPhamTram(data.data.KyTruocTienGiamChiaDoanhThu),
                        KyBaoCaoTienGiamChiaDoanhThu: NonPhamTram(data.data.KyBaoCaoTienGiamChiaDoanhThu),
                        ThaydoiTienGiamChiaDoanhThu: ColorPhanTram(data.data.ThaydoiTienGiamChiaDoanhThu),
                        LaiGopChiaDoanhThu: '8.' + data.data.LaiGopChiaDoanhThu,
                        KyTruocTienGiamChiaLaiGop: NonPhamTram(data.data.KyTruocTienGiamChiaLaiGop),
                        KyBaoCaoTienGiamChiaLaiGop: NonPhamTram(data.data.KyBaoCaoTienGiamChiaLaiGop),
                        ThaydoiLaiGopChiaDoanhThu: ColorPhanTram(data.data.ThaydoiLaiGopChiaDoanhThu)
                    });
                    $('#tbtheobaocao').html(html);
                }
            }
        });
        function Non(price) {
            return '<th style="text-align:right">' + new Intl.NumberFormat().format(price) + '</th>';
        }
        function Color(price) {
            if (price > 0) {
                return '<th style="text-align:right;color:blue">' + new Intl.NumberFormat().format(price) + '</th>';
            } else if (price < 0) {
                return '<th style="text-align:right;color:red">' + new Intl.NumberFormat().format(price) + '</th>';
            } else {
                return '<th style="text-align:right">' + new Intl.NumberFormat().format(price) + '</th>';
            }

        }

        function NonPhamTram(price) {
            return '<th style="text-align:right">' + new Intl.NumberFormat().format(price) + '%</th>';
        }
        function ColorPhanTram(price) {
            if (price > 0) {
                return '<th style="text-align:right;color:blue">' + new Intl.NumberFormat().format(price) + '%</th>';
            } else if (price < 0) {
                return '<th style="text-align:right;color:red">' + new Intl.NumberFormat().format(price) + '%</th>';
            } else {
                return '<th style="text-align:right">' + new Intl.NumberFormat().format(price) + '%</th>';
            }

        }

        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')}`
        }

    },
    searchthongketbaocao: function () {
        $("#btnTiemKiemTheoBaoCao").off('click').on('click', function (e) {
            e.preventDefault();
            StatisticLNController.baocao();
        })
    },
    ExportBaoCao: function () {
        $("#ExportExcelBaoCao").off('click').on('click', function () {

            var search = new Object();
            search.TuNgay = $("#TuNgayThongKeBaoCao").val();
            search.DenNgay = $("#DenNgayThongKeBaoCao").val();

            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/StatisticLN/ExportExcelBaoCao',
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
    },

}
StatisticLNController.init();