
var pg = {
    pageSize: 3,
    pageIndex: 1
}
var pgdetail = {
    pageSize: 3,
    pageIndex: 1
}

var pgtopk = {
    pageSize: 3,
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
var productcomboController = {
    init: function () {
        //Khởi tạo theme
        productcomboController.initTheme();
        //Show danh dánh sản phẩm combo
        productcomboController.lstproductcombo();

        //Thêm sản phẩm combo
        productcomboController.addProductCombo();


        //Search nân cao
        productcomboController.searchNangCap();

        productcomboController.searchNangCapDetail();

        productcomboController.SearchProductCombo();


        //Thêm sản phẩm vào combo
        productcomboController.addproduct();


        productcomboController.listProductAddCombo();


        productcomboController.readFiletxt();
    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    chooseImageProduct: function () {
        $('#ImageProductChoose').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageProduct').val(url);
                $("#ImageShowProduct").removeAttr('src');
                $("#ImageShowProduct").attr('src', url);
            };
            finder.popup();
        });
    },
    chooseImageMoreProduct: function () {
        $('#ImageMoreProductChoose').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageList').append('<div class="col-2"><img src="' + url + '" width="100" /><a href="#" class="btndelete" style="margin-left: 42px;"><i class="fas fa-trash" ></i></a>' + '<input type ="hidden" class="hidImage" value ="' + url + '"></div>');

                $('.btndelete').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                })
            };
            finder.popup();
        });
    },
    addProductCombo: function () {
        productcomboController.chooseImageProduct();
        productcomboController.chooseImageMoreProduct();
        $('#CreateSanPhamCombo').off('click').on('click', function () {
            $('#ModalCreateSPCombo').modal('show');
            CKEDITOR.replace('DiscriptionProduct', {
                customConfig: '/Assets/Admin/plugins/ckeditor/config.js'
            });
            $("#FromCreateProductCombo").off('submit').on('submit', function (e) {

                e.preventDefault();

                var imagemore = [];

                $.each($('#ImageList .hidImage'), function (i, item) {
                    imagemore.push($(item).val());
                });


                var product = new Object();
                var txt = CKEDITOR.instances['DiscriptionProduct'].getData();
                product.Name = $("#NameProduct").val();
                product.StartDate = $("#StartDate").val();
                product.EndDate = $("#EndDate").val();
                product.CountProduct = $("#CountCombo").val();
                product.DisplayProductComBo = $("#DisplayCombo").val();
                product.PriceOut = $("#PriceOutProduct").val().split('.').join("");
                product.Pricewholesale = $("#PricewholesaleProduct").val().split('.').join("");
                product.Image = $("#ImageProduct").val();
                product.MoreImage = JSON.stringify(imagemore);
                product.Discription = txt;
                product.TradeMark = $("#TradeMarkProduct").val();
                product.TradeOrigin = $("#TradeOriginProduct").val();
                product.Ingredient = $("#IngredientProduct").val();
                product.Production = $("#ProductionProduct").val();
                product.Expiry = $("#ExpiryProduct").val();
                product.UserManual = $("#UserManualProduct").val();
                product.CareInstructions = $("#CareInstructionsProduct").val();
                product.Packing = $("#PackingProduct").val();
                $.ajax({
                    type: "POST",
                    url: '/admin/productcombo/AddProduct',
                    data: product,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công!.',
                                })
                                $('#paginationProductcombo').twbsPagination('destroy');
                                $("#NameProduct").val('');
                                $("#StartDate").val('');
                                $("#EndDate").val('');
                                $("#CountCombo").val('');
                                $("#PriceOutProduct").val('');
                                $("#PricewholesaleProduct").val('');
                                $("#UnitProduct").val('');
                                $("#productcategory").val('');
                                $("#TradeMarkProduct").val('');
                                $("#TradeOriginProduct").val('');
                                $("#IngredientProduct").val('');
                                $("#ProductionProduct").val('');
                                $("#ExpiryProduct").val('');
                                $("#UserManualProduct").val('');
                                $("#CareInstructionsProduct").val('');
                                $("#PackingProduct").val('');
                                CKEDITOR.instances['DiscriptionProduct'].setData('');
                                $("#ImageShowProduct").removeAttr('src');
                                $("#ImageProduct").val('');
                                $('#ModalCreateSPCombo').modal('hide');

                                productcomboController.lstproductcombo()
                            } else {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thất bại',
                                })
                            }
                        }
                    }
                });


            });

        })
    },
    actionDisplayProduct: function () {
        var flag = false;
        $('.actiondisplayProduct').off('click').on('click', function () {
            var id = $(this).data('id');
            console.log('da click' + id);
            if (flag == false) {
                $('#actiondisplay').toggle('1000');
                icon = $(this).find("i");
                icon.toggleClass("fas fa-play fas fa-pause")
                flag = true;
            }
            else {
                $('#actiondisplay').hide('1000');
                $('#actiondisplay').toggle('1000');
                icon = $(this).find("i");
                icon.toggleClass("fas fa-pause fas fa-play")
                flag = false;
            }

            $.ajax({
                type: "get",
                url: '/admin/product/ActionProduct',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data == true) {
                            Toast.fire({
                                icon: 'success',
                                title: 'Sản phẩm đang bán',
                            })
                        } else {
                            Toast.fire({
                                icon: 'success',
                                title: 'Ngừng bán sản phẩm',
                            })
                        }
                    }
                }
            });


        })
    },
    lstproductcombo: function (changePageSize) {

        var search = new Object();
        search.querysearch = $("#search-querysearch").val();
        search.TinhTrangCombo = $("#search-TinhTrang").val();
        search.TrangThaiCombo = $("#search-TrangThai").val();
        search.SoLuong = $("#search-SoLuong").val();
        search.GiaLeTu = $("#GiaLeTu").val().split('.').join("");
        search.GiaLeDen = $("#GiaLeDen").val().split('.').join("");
        search.GiaSiTu = $("#GiaSiTu").val().split('.').join("");
        search.GiaSiDen = $("#GiaSiDen").val().split('.').join("");

        $.ajax({
            url: '/admin/productcombo/ListProductComo',
            type: 'post',
            data: {
                page: pg.pageIndex,
                pageSize: pg.pageSize,
                search:search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templateProductcombo').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ProductComboIDDisplay: 'CB' +ChangeIDProduct(value.ProductComboID),
                            ProductComboID: value.ProductComboID,
                            ProductID: value.ProductID,
                            Name: value.Name,
                            CreateDate: parseJsonDate(value.CreateDate),
                            DisplayProductComBo: value.DisplayProductComBo,
                            PriceOut: new Intl.NumberFormat().format(value.PriceOut),
                            PricePromotion: new Intl.NumberFormat().format(value.PricePromotion),
                            Pricewholesale: new Intl.NumberFormat().format(value.Pricewholesale),
                            PricewholesalePromotion: new Intl.NumberFormat().format(value.PriceOut),
                            StartDate: parseJsonDate(value.StartDate),
                            EndDate: parseJsonDate(value.EndDate),
                            CountProduct: value.CountProduct,
                            ComboStatus: Status(value.ComboStatus),
                            DisplayProduct: value.DisplayProduct ? "fas fa-pause" : "fas fa-play"
                        });
                    });
                    $('#tbProductcombo').html(html);

                    if (data.total != 0) {
                        productcomboController.paging(data.total, function () {
                            productcomboController.lstproductcombo();
                        }, changePageSize)
                    }

                    productcomboController.detailcombo();
                    productcomboController.actionDisplayProduct();
                }
            }
        });
        function Status(value) {
            if (value == true) {
                return "<span class=\"badge bg-primary\">Đang hoạt động</span>";
            }
            if (value == false) {
                return "<span class=\"badge bg-danger\">Dừng</span>";
            }          
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
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    detailcombo: function () {
        $('.modalshowsub').on('click', function () {
            var id = $(this).data('id');
            var start = $(this).data('start');
            var end = $(this).data('end');
            $("#ComBoIDDisplay").val('CB' + ChangeIDProduct(id));
            $("#ComBoID").val(id);
            $("#ComboStartDate").val(start);
            $("#ComboEndDate").val(end);

            productcomboController.listProduct(id);

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
    listProduct: function (id,changePageSize) {

        
        $.ajax({
            url: '/admin/productcombo/ListProductComoDetail',
            type: 'post',
            data: {
                id:id,
                page: pgdetail.pageIndex,
                pageSize: pgdetail.pageSize,
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templateProduct').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.ProductID,
                            Unit: value.ParentProductID,
                            ProductID: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.ProductID),
                            ProductName: value.Name,
                            UnitName: value.UnitName,
                            Count: value.CountProduct,
                            PriceInput: new Intl.NumberFormat().format(value.PriceIput),
                            PriceOutput: new Intl.NumberFormat().format(value.PriceOut),
                            PricePromotion: new Intl.NumberFormat().format(value.PricePromotion),
                            Pricewholesale: new Intl.NumberFormat().format(value.Pricewholesale),
                            PricewholesalePromotion: new Intl.NumberFormat().format(value.PricewholesalePromotion),
                            SupplierName: value.SupplierName,
                            CategoryName: value.CategoryName.length > 28 ? value.CategoryName.substr(0, 28) : value.CategoryName,
                            DisplayProduct: value.Display == true ? "fas fa-pause" : "fas fa-play",
                            IconQuyDoi: QuyDoiDVT(value.CountProduct, value.ParentProductID)
                        });
                    });
                    $('#tbProduct').html(html);

                    if (data.total != 0) {
                        productcomboController.pagingDetail(data.total, function () {
                            productcomboController.listProduct(id);
                        }, changePageSize)
                    }
                }
            }
        });
        function QuyDoiDVT(count, dvt) {
            if (count != 0 && dvt != '0') {
                return "fas fa-exchange-alt actionQUYDOI";
            }
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
    pagingDetail: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgdetail.pageSize)
        if ($('#paginationproduct a').length === 0 || changePageSize === true) {
            $('#paginationproduct').empty();
            $('#paginationproduct').removeData('twbs-pagination');
            $('#paginationproduct').unbind("page");
        }
        $('#paginationproduct').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgdetail.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    searchNangCap: function () {
        $("#btnSearchNangCao").off('click').on('click', function () {
            $('#SearchNangCao').toggle();;


        })

    },
    searchNangCapDetail: function () {
        $("#btnSearchNangCaodetail").off('click').on('click', function () {
            $('#SearchNangCaoDetail').toggle();;


        })

    },
    SearchProductCombo: function () {

        $("#BtnSearchProductCombo").off('click').on('click', function () {
            $('#paginationProductcombo').twbsPagination('destroy');
            productcomboController.lstproductcombo();
        })
    },
    addproduct: function () {
        $("#AddProductCombo").off('click').on('click', function () {

            var check = $("#ComBoID").val();

            if (check == "") {
                Toast.fire({
                    icon: 'error',
                    title: 'Chưa chọn combo để thêm!.',
                })
            } else {
                $.ajax({
                    url: '/admin/productcombo/AddProductCombo',
                    type: 'post',
                    data: {
                        idcombo: check,
                        idproduct: $('#productcomboadd').val()
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code = 200) {
                            if (data.data) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công!.',
                                })
                                productcomboController.listProduct(check);

                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Sản phẩm đã có!.',
                                })
                            }
                        }
                    }
                });       
            }

        })
    },
    listProductAddCombo: function () {
        $.ajax({
            url: '/admin/product/ListProductAddCombo',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#productcomboadd');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.ProductID).text(val.Name + ' - ĐVT: ' + val.UnitName).appendTo($select);
                    });
                }
            }
        });
    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#paginationProductcombo a').length === 0 || changePageSize === true) {
            $('#paginationProductcombo').empty();
            $('#paginationProductcombo').removeData('twbs-pagination');
            $('#paginationProductcombo').unbind("page");
        }
        $('#paginationProductcombo').twbsPagination({
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

    readFiletxt: function () {
        $("#readfile").off('click').on('click', function () {

            $.ajax({
                type: "get",
                url: '/admin/tkualgo/ReadFile',
                data: {
                    output: $("#outputpath").val()
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.status == true) {
                            var html = "";
                            var template = $('#data-templatetopk').html();
                            $.each(data.data, function (key, value) {
                                html += Mustache.render(template, {
                                    STT: value.STT,
                                    PU: new Intl.NumberFormat().format(value.PU),
                                });
                            });
                            $('#tbtopk').html(html);
                            Toast.fire({
                                icon: 'success',
                                title: 'Đọc file thành công',
                            })


                            productcomboController.DetailProduct(data.data);

                        } else {
                            Toast.fire({
                                icon: 'error',
                                title: 'File không đúng định dạng',
                            })
                        }

                    }
                }
            });

        })
    },
    DetailProduct: function (lst) {

        $(".actionViewTOPKDetail").off('click').on('click', function () {
            $("#Modaldetailtopk").modal('show');
            var stt = $(this).data('id');
            console.log(lst)

            $.each(lst, function (key, value) {

                if (value.STT == stt) {

                    var id = [];
                    $.each(value.MProduct, function (key, value) {

                        id.push(value.ProductID);
                    });

                    $.ajax({
                        type: "POST",
                        url: '/admin/tkualgo/GetProducts',
                        data: {
                            id: id
                        },
                        dataType: 'json',
                        success: function (data) {
                            if (data.code == 200) {
                                var html = "";
                                var template = $('#data-templateProducttopk').html();
                                $.each(data.data, function (key, value) {
                                    html += Mustache.render(template, {
                                        STT: value.STT,
                                        ID: value.ProductID,
                                        Unit: value.ParentProductID,
                                        ProductID: 'SP' + value.KyTuCaterory + ChangeIDProduct(value.ProductID),
                                        ProductName: value.Name,
                                        UnitName: value.UnitName,
                                        Count: value.CountProduct,
                                        PriceInput: new Intl.NumberFormat().format(value.PriceIput),
                                        PriceOutput: new Intl.NumberFormat().format(value.PriceOut),
                                        PricePromotion: new Intl.NumberFormat().format(value.PricePromotion),
                                        Pricewholesale: new Intl.NumberFormat().format(value.Pricewholesale),
                                        PricewholesalePromotion: new Intl.NumberFormat().format(value.PricewholesalePromotion),
                                        SupplierName: value.SupplierName,
                                        CategoryName: value.CategoryName.length > 28 ? value.CategoryName.substr(0, 28) : value.CategoryName,
                                        DisplayProduct: value.Display == true ? "fas fa-pause" : "fas fa-play",
                                        IconQuyDoi: QuyDoiDVT(value.CountProduct, value.ParentProductID)
                                    });
                                });
                                $('#tbProducttopk').html(html);
                                productcomboController.addlistcombo()
                            }
                        }
                    });
                }
            });

        })
        function QuyDoiDVT(count, dvt) {
            if (count != 0 && dvt != '0') {
                return "fas fa-exchange-alt actionQUYDOI";
            }
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

    addlistcombo: function () {

        $("#addproductincombo").off('click').on('click', function () {

            var names = [];
            $('#bodyproduct input:checked').each(function () {
                names.push(this.name);
            });

            var check = $("#ComBoID").val();

            if (check == "") {
                Toast.fire({
                    icon: 'error',
                    title: 'Chưa chọn combo để thêm!.',
                })
            } else {
                $.ajax({
                    url: '/admin/productcombo/AddProductComboList',
                    type: 'post',
                    data: {
                        idcombo: check,
                        idproduct: names
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code = 200) {
                            if (data.data) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công!.',
                                })
                                productcomboController.listProduct(check);

                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Sản phẩm đã có | chưa chọn sản phẩm | sản phẩm đã hết hàng!.',
                                })
                            }
                        }
                    }
                });
            }
        })
    },
}
productcomboController.init();

