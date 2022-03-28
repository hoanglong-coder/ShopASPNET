
var pg = {
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
var OrderController = {
    init: function () {
        //Khởi tạo theme
        OrderController.initTheme();
        //Show danh dánh account
        OrderController.listOrder();

        //search order
        OrderController.searchOrder();

        //Export ListOrder
        OrderController.ExportDSHoaDon();

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    searchOrder: function () {
        $("#BtnSearchOrder").off('click').on('click', function (e) {
            e.preventDefault();
            $('#pagination').twbsPagination('destroy');
            OrderController.listOrder();

        })
    },
    listOrder: function (changePageSize) {

        var search = new Object();
        search.query = $("#search-querysearch").val();
        search.TuNgay = $("#search-TuNgay").val();;
        search.DenNgay = $("#search-DenNgay").val();;
        search.TrangThai = $("#search-Trangthai").val();;
        search.PTThanhToan = $("#search-PTThanhtoan").val();;

        $.ajax({
            url: '/admin/order/ListOrder',
            type: 'post',
            data: {
                page: pg.pageIndex,
                pageSize: pg.pageSize,
                search:search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {          
                    var html = '';
                    var template = $('#data-template').html();
                    let totalprice = data.totalprice;
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            ID: value.OrderID,
                            DisplayID: 'HD' + ChangeIDProduct(value.OrderID),
                            Name: value.ShipName,
                            Phone: value.ShipPhone,
                            Address: value.ShipAddress,
                            Email: value.ShipEmail,
                            Totalprice: new Intl.NumberFormat().format(value.TotalPrice),
                            TotalCount: value.TotalCount,
                            CREATEDATE: parseJsonDate(value.CreateDate),
                            Email: value.ShipEmail,
                            Status: Status(value.OrderStatus),
                            UserName: value.UserName,
                            PaymentStatus:value.PaymentName,
                            st: value.OrderStatus,
                            Ghichu: value.Discription
                        });
                    });
                    $('#tbOrder').html(html);

                    $("#CountProduct").empty();
                    $("#CountProduct").append(data.totalcout);
                    $("#TotalPriceOrder").empty();
                    $("#TotalPriceOrder").append(new Intl.NumberFormat().format(data.totalprice));

                    OrderController.paging(data.total, function () {
                        OrderController.listOrder();
                    }, changePageSize)
                    OrderController.listOrderDetail();
                    OrderController.ExportExcelHoaDon();
                    
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

            return `${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getDate().toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
        function Status(value) {
            if (value == -1) {
                return "<span class=\"badge bg-danger\">Chưa xác nhận</span>";
            }
            if (value == 0) {
                return "<span class=\"badge bg-primary\">Đã xác nhận</span>";
            }
            if (value == 1) {
                return "<span class=\"badge bg-warning\">Đang giao hàng</span>"
            }
            if (value == 2) {
                return "<span class=\"badge bg-success\">Hoàn thành</span>"
            }
        }
    },
    listOrderDetail: function () {
        $('.modalshowsub').on('click', function () {
            
            var id = $(this).data('id');
            var status = $(this).data('status');
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
                                ID: value.Product.ProductID.length > 8 ? value.Product.ProductID.substr(0, 8) : value.Product.ProductID,
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
                        $('#KhuyenMai').append('-'+new Intl.NumberFormat().format(data.khuyenmai));

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
                        if (status == -1) {
                            $(".btn-change-status").removeClass("btn btn-warning btn btn-success").addClass("btn btn-primary");
                            $(".btn-change-status").text("Xác nhận đơn hàng");
                            
                            $(".btn-change-status").off('click').on('click', function () {
                                var check = $(".btn-change-status").text();
                                if (check == "Xác nhận đơn hàng") {
                                    $.ajax({
                                        url: '/admin/order/OrderConfirma',
                                        type: 'post',
                                        data: {
                                            idorder: id
                                        },
                                        dataType: 'json',
                                        success: function (data) {
                                            if (data.code == 200) {
                                                $('#suborderdetail').modal('hide');
                                                Toast.fire({
                                                    icon: 'success',
                                                    title: 'Xác nhận đơn hàng thành công!.',
                                                })
                                                OrderController.listOrder(true);
                                            }
                                        }
                                    })
                                }                              
                            })                           
                        }
                        if (status == 0) {
                            $(".btn-change-status").removeClass("btn btn-primary").addClass("btn btn-warning");
                            $(".btn-change-status").text("Đã xác nhận");
                        }
                        if (status == 1) {
                            $(".btn-change-status").removeClass("btn btn-primary btn btn-warning").addClass("btn btn-warning");
                            $(".btn-change-status").text("Đang giao hàng");
                        }
                        if (status == 2) {
                            $(".btn-change-status").removeClass("btn btn-primary btn btn-warning").addClass("btn btn-success");
                            $(".btn-change-status").text("Hoàn thành");
                        }
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
    createAccount: function () {

        $('.btn-create').off('click').on('click', function () {
            if ($('#frmCreate').valid()) {
                var user = new Object();
                user.UserName = $("#inputUserNameCreate").val();
                user.Password = $("#inputPasswordCreate").val();
                user.Name = $("#inputNameCreate").val();
                user.Email = $("#inputEmailCreate").val();
                user.Address = $('#tinhCreate').val();
                user.Phone = $('#inputPhoneCreate').val();
                user.Status = $("[name='statusCreate']").prop('checked');
                if (user != null) {
                    $.ajax({
                        url: '/admin/user/create',
                        type: 'post',
                        datatype: 'json',
                        data: user,
                        success: function (data) {
                            if (data.code == 200) {
                                $('#modal-create').modal('hide');
                                $(':input', '#modal-create').not('#accountType').val('').removeAttr('checked')
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công!.',
                                })
                                OrderController.listAccount(true);
                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Thêm thất bại!.',
                                })
                            }
                        }
                    });
                }
            }
        });
    },
    ExportExcelHoaDon() {
        $(".reportexcel").off('click').on('click', function () {
            var id = $(this).data('id');
            $.ajax({
                url: '/admin/order/ExportExcelOrder',
                type: 'post',
                data: {
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

        $(".reportpdf").off('click').on('click', function () {
            var id = $(this).data('id');
            Toast.fire({
                icon: 'success',
                title: 'Chờ một lát',
            })
            $.ajax({
                url: '/admin/order/ExportPDFOrder',
                type: 'post',
                data: {
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
    ExportDSHoaDon() {
        $("#ExportExcelOrder").off('click').on('click', function () {
            var search = new Object();
            search.query = $("#search-querysearch").val();
            search.TuNgay = $("#search-TuNgay").val();
            search.DenNgay = $("#search-DenNgay").val();
            search.TrangThai = $("#search-Trangthai").val();
            search.PTThanhToan = $("#search-PTThanhtoan").val();
            $.ajax({
                url: '/admin/order/ExportExcelListOrder',
                type: 'post',
                data: search,
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
        $("#ExportPDFOrder").off('click').on('click', function () {
            var search = new Object();
            search.query = $("#search-querysearch").val();
            search.TuNgay = $("#search-TuNgay").val();
            search.DenNgay = $("#search-DenNgay").val();
            search.TrangThai = $("#search-Trangthai").val();
            search.PTThanhToan = $("#search-PTThanhtoan").val();
            $.ajax({
                url: '/admin/order/ExportPDFListOrder',
                type: 'post',
                data: search,
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
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#pagination a').length === 0 || changePageSize === true) {
            $('#pagination').empty();
            $('#pagination').removeData('twbs-pagination');
            $('#pagination').unbind("page");
        }
        $('#pagination').twbsPagination({
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
}
OrderController.init();

