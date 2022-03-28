
var pg = {
    pageSize: 2,
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
var ReceiptController = {
    init: function () {
        //Khởi tạo theme
        ReceiptController.initTheme();

        //thêm sản phẩm vào phiếu nhập
        ReceiptController.AddProductReceipt();

        //Danh sách nhà sảm xuất
        ReceiptController.listSupplier();

        //Tạo phiếu nhập
        ReceiptController.AddReceipt();

        //Danh sách phiếu nhập
        ReceiptController.lstReceipt();

        //Tìm kiếm
        ReceiptController.searchReceipt();

        //Xuất Export
        ReceiptController.ExportReceipt();

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    AddProductReceipt: function () {
        ReceiptController.listProduct();
            $("#FromAddProduct").off('submit').on('submit', function (e) {

                e.preventDefault();
                $.ajax({
                    type: "POST",
                    url: '/admin/receipt/AddProductReceipt',
                    data: {
                        idproduct: $("#ProductAll").val(),
                        inputprice: $("#PriceInput").val().split('.').join(""),
                        countproduct: $("#CountProduct").val()

                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.flag == true) {
                                var html = "";
                                var template = $('#data-templateProductReciept').html();
                                var tongtien = 0;
                                $.each(data.data, function (key, value) {
                                    html += Mustache.render(template, {
                                        STT: value.STT,
                                        ProductID: value.Productid,
                                        ProductIDDisplay: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.Productid),
                                        PriceIput: new Intl.NumberFormat().format(value.PriceIput),
                                        ReceiptCount: value.ReceiptCount,
                                        ProductName: value.ProductName,
                                        TotalPrice: new Intl.NumberFormat().format(value.Total)
                                    });
                                    tongtien += value.Total;
                                });
                                $("#TotalPrice").empty();
                                $("#TotalPrice").val(new Intl.NumberFormat().format(tongtien));
                                $('#tbProductReceipt').html(html);
                                $("#CountProduct").val('');
                                $("#PriceInput").val('')
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công',
                                })
                                ReceiptController.DeleteProductReceipt();
                                ReceiptController.editProductReceipt();
                            } else {
                                var html = "";
                                var template = $('#data-templateProductReciept').html();
                                $.each(data.data, function (key, value) {
                                    html += Mustache.render(template, {
                                        STT: value.STT,
                                        ProductID: value.Productid,
                                        ProductIDDisplay: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.Productid),
                                        PriceIput: new Intl.NumberFormat().format(value.PriceIput),
                                        ReceiptCount: value.ReceiptCount,
                                        ProductName: value.ProductName,
                                        TotalPrice: new Intl.NumberFormat().format(value.Total)
                                    });
                                });
                                $('#tbProductReceipt').html(html);


                                Toast.fire({
                                    icon: 'error',
                                    title: 'Trùng sản phẩm',
                                })
                                ReceiptController.DeleteProductReceipt();
                                ReceiptController.editProductReceipt();

                            }                                                      
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
    DeleteProductReceipt: function () {
        $(".actiondelete").off('click').on('click', function (e) {
            var id = $(this).data('id');

            e.preventDefault();
            $.ajax({
                type: "POST",
                url: '/admin/receipt/DeleteProductReceipt',
                data: {
                    idproduct:id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.flag == true) {
                            var html = "";
                            var template = $('#data-templateProductReciept').html();
                            var tongtien = 0;
                            $.each(data.data, function (key, value) {
                                html += Mustache.render(template, {
                                    STT: value.STT,
                                    ProductID: value.Productid,
                                    ProductIDDisplay: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.Productid),
                                    PriceIput: new Intl.NumberFormat().format(value.PriceIput),
                                    ReceiptCount: value.ReceiptCount,
                                    ProductName: value.ProductName,
                                    TotalPrice: new Intl.NumberFormat().format(value.Total)
                                });
                                tongtien += value.Total;
                            });
                            $("#TotalPrice").empty();
                            $("#TotalPrice").val(new Intl.NumberFormat().format(tongtien));
                            $('#tbProductReceipt').html(html);
                            $("#CountProduct").val('');
                            $("#PriceInput").val('')
                            Toast.fire({
                                icon: 'success',
                                title: 'Xóa thành công',
                            })
                            ReceiptController.DeleteProductReceipt();
                            ReceiptController.editProductReceipt();
                        }
                    }
                }
            });


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
    editProductReceipt: function () {
        $(".actioneEdit").off('click').on('click', function () {
            var id = $(this).data('id');
            $('#Addproduct').attr('disabled', 'disabled');
            $('#Updateproduct').removeAttr('disabled');

            $.ajax({
                type: "get",
                url: '/admin/receipt/DetailProductReceipt',
                data: {
                    idproduct: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        console.log(id)
                        $("#ProductAll").val(id).change();
                        $("#PriceInput").val(new Intl.NumberFormat().format(data.data.PriceIput));
                        $("#CountProduct").val(data.data.ReceiptCount);
                    }
                }
            });


            $("#FromAddProduct").off('submit').on('submit', function (e) {
                e.preventDefault();
                $('#Addproduct').removeAttr('disabled');
                $('#Updateproduct').attr('disabled', 'disabled');
                $.ajax({
                    type: "POST",
                    url: '/admin/receipt/EditProductReceipt',
                    data: {
                        idproduct: $("#ProductAll").val(),
                        inputprice: $("#PriceInput").val().split('.').join(""),
                        countproduct: $("#CountProduct").val()

                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.flag == true) {
                                var html = "";
                                var template = $('#data-templateProductReciept').html();
                                var tongtien = 0;
                                $.each(data.data, function (key, value) {
                                    html += Mustache.render(template, {
                                        STT: value.STT,
                                        ProductID: value.Productid,
                                        ProductIDDisplay: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.Productid),
                                        PriceIput: new Intl.NumberFormat().format(value.PriceIput),
                                        ReceiptCount: value.ReceiptCount,
                                        ProductName: value.ProductName,
                                        TotalPrice: new Intl.NumberFormat().format(value.Total)
                                    });
                                    tongtien += value.Total;
                                });
                                $("#TotalPrice").empty();
                                $("#TotalPrice").val(new Intl.NumberFormat().format(tongtien));
                                $('#tbProductReceipt').html(html);
                                $("#CountProduct").val('');
                                $("#PriceInput").val('')
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Cập nhật thành công',
                                })
                                ReceiptController.DeleteProductReceipt();
                                ReceiptController.editProductReceipt();
                                ReceiptController.AddProductReceipt();
                            } else {
                                var html = "";
                                var template = $('#data-templateProductReciept').html();
                                $.each(data.data, function (key, value) {
                                    html += Mustache.render(template, {
                                        STT: value.STT,
                                        ProductID: value.Productid,
                                        ProductIDDisplay: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.Productid),
                                        PriceIput: new Intl.NumberFormat().format(value.PriceIput),
                                        ReceiptCount: value.ReceiptCount,
                                        ProductName: value.ProductName,
                                        TotalPrice: new Intl.NumberFormat().format(value.Total)
                                    });
                                });
                                $('#tbProductReceipt').html(html);


                                Toast.fire({
                                    icon: 'error',
                                    title: 'Trùng sản phẩm',
                                })
                                ReceiptController.DeleteProductReceipt();
                                ReceiptController.editProductReceipt();
                                ReceiptController.AddProductReceipt();

                            }
                        }
                    }
                });
            })
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
    AddReceipt: function () {
        $("#FromCreateReceipt").off('submit').on('submit', function (e) {
            e.preventDefault()
            var Receipt = new Object();
            Receipt.SupplierID = $("#LstSupplier").val();
            Receipt.CreateDate = $("#CreateDateReceipt").val();
            Receipt.Description = $("#DescriptionReceipt").val();

            $.ajax({
                type: "POST",
                url: '/admin/receipt/CreateReceipt',
                data: Receipt,
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.flag == true) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công',
                                })
                                $("#CreateDateReceipt").val('');

                                $("#DescriptionReceipt").val('');

                                $('#tbl-dataProduct tbody').empty();

                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Thêm thất bại',
                                })
                            }                           
                        }else if(data.flag==false) {
                            Toast.fire({
                                icon: 'error',
                                title: 'Chưa nhập sản phẩm vào phiếu nhập',
                            })
                        }
                    }
                }
            });


        });


    },
    listProduct: function () {
        $.ajax({
            url: '/admin/product/ListProductThemPhieuNhap',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#ProductAll');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.ProductID).text('SP' + val.KyTuCaterory + ChangeIDProduct(val.ProductID)+' - ' + val.Name).appendTo($select);
                    });
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
    listSupplier: function () {
        $.ajax({
            url: '/admin/supplier/ListSupplier',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#LstSupplier');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.SupplierID).text(val.Name + ' - NCC' + ChangeIDProduct(val.SupplierID)).appendTo($select);
                    });
                    $(`#LstSupplier option[value='${1}']`).prop('selected', true);
                }
            }
        });
        $.ajax({
            url: '/admin/userlogin/GetUser',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    $("#UserName").val(data.data.FullName);
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
    lstReceipt: function (changePageSize) {
        var search = new Object();
        search.SearchName = $("#search-receipt").val();
        search.TuNgay = $("#TuNgayreceipt").val();
        search.DenNgay = $("#DenNgayreceipt").val();


        $.ajax({
            url: '/admin/receipt/GetAll',
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
                    var template = $('#data-templateReceipt').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ReceiptID: value.ReceiptID,
                            ReceiptIDDisplay: 'PN' +ChangeIDProduct(value.ReceiptID),
                            CreateDate: parseJsonDate(value.CreateDate),
                            SupplierName: value.SupplierName,
                            CountProduct: new Intl.NumberFormat().format(value.TotalCount),
                            TotalPriceInput: new Intl.NumberFormat().format(value.TotalReceiptPrice),
                            UserName: value.UserName
                            
                        });
                    });
                    $('#tbReceipt').html(html);
                    if (data.total != 0) {
                        ReceiptController.paging(data.total, function () {
                            ReceiptController.lstReceipt();
                        }, changePageSize)
                    }

                    $('#Count').empty();
                    $('#Count').append(data.TotalCount);

                    $('#TotalPriceReceipt').empty();
                    $('#TotalPriceReceipt').append(new Intl.NumberFormat().format(data.Totalprice));

                    ReceiptController.ReceiptDetail();
                    ReceiptController.ExportReceiptDetail();

                }
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

        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getDate().toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    searchReceipt: function () {
        $("#btnreceipt").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationReceipt').twbsPagination('destroy');
            ReceiptController.lstReceipt();
        })

    },
    ReceiptDetail: function () {
        $(".actionViewReceiptDetail").off('click').on('click', function () {

            var id = $(this).data('id');           
            $.ajax({
                url: '/admin/receipt/GetReceiptDetailsByID',
                type: 'post',
                data: {
                    id:id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        $("#headerReceiptDetail").empty()
                        $("#headerReceiptDetail").append(data.SupplierName + " - Mã phiếu nhập:" + 'PN' + ChangeIDProduct(id));

                        $("#bottomReceiptDetail").empty();
                        $("#bottomReceiptDetail").append('Ngày nhập:' + parseJsonDate(data.CreateDate) + ' - Người nhập:' + data.NameUser);
                       
                        var html = "";
                        var template = $('#data-templateReceiptDetail').html();
                        $.each(data.data, function (key, value) {
                            html += Mustache.render(template, {
                                STT: value.STT,
                                ProductIDDisplay: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.ProductID),
                                ProductName: value.ProductName,
                                ProductDVT: value.ProductDVT,
                                CountProduct: value.ReceiptCount,
                                PriceInput: new Intl.NumberFormat().format(value.PriceIput),
                                TotalPrice: new Intl.NumberFormat().format(value.TotalPrice),
                            });
                        });
                        $('#tbReceiptDetail').html(html);

                        $("#TongSoluongReceiptDetail").empty();
                        $("#TongSoluongReceiptDetail").append(data.CountProduct);

                        $("#TotalPriceReceiptDetail").empty();
                        $("#TotalPriceReceiptDetail").append(new Intl.NumberFormat().format(data.TotalPrice));


                        $("#ModalViewReceiptDetail").modal('show');
                    }
                }

            })

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
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getDate().toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    ExportReceiptDetail: function () {
        $(".reportexcel").off('click').on('click', function () {
            var id = $(this).data('id');
            $.ajax({
                url: '/admin/receipt/ExportExcelProductReceipt',
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
            $.ajax({
                url: '/admin/receipt/ExportPDFProductReceipt',
                type: 'post',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        if (data.NameFile !== '') {
                            Toast.fire({
                                icon: 'success',
                                title: 'Chờ một lát',
                            })
                            location.href = "/Data/ExportPDF/" + data.NameFile;
                            
                        }
                    }
                }

            })
        })
    },
    ExportReceipt: function () {
        $("#ExportExcelReceipt").off('click').on('click', function () {
            var search = new Object();
            search.SearchName = $("#search-receipt").val();
            search.TuNgay = $("#TuNgayreceipt").val();
            search.DenNgay = $("#DenNgayreceipt").val();
            $.ajax({
                url: '/admin/receipt/ExportExcelReceipt',
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
        $("#ExportPDFReceipt").off('click').on('click', function () {
            var search = new Object();
            search.SearchName = $("#search-receipt").val();
            search.TuNgay = $("#TuNgayreceipt").val();
            search.DenNgay = $("#DenNgayreceipt").val();
            $.ajax({
                url: '/admin/receipt/ExportPDFReceipt',
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
        if ($('#paginationReceipt a').length === 0 || changePageSize === true) {
            $('#paginationReceipt').empty();
            $('#paginationReceipt').removeData('twbs-pagination');
            $('#paginationReceipt').unbind("page");
        }
        $('#paginationReceipt').twbsPagination({
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
ReceiptController.init();