
var pg = {
    pageSize: 10,
    pageIndex: 1
}
var pgpromotion = {
    pageSize: 6,
    pageIndex: 1
}
var pgpromotionDVT = {
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
var ProductController = {
    init: function () {
        //Khởi tạo theme
        ProductController.initTheme();

        //Show danh sách sản phẩm
        ProductController.listProduct();

        //Create sản phẩm
        ProductController.addProduct();

        //Create sản phảm khác đơn vị tính
        ProductController.addProductDVT();

        //Search
        ProductController.SearchProduct();


        //Load Loại sản phẩm
        ProductController.SearchLstSelecttCategory();


        //SearchNangCao
        ProductController.searchNangCap();
        

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
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
                        if (data.data.Check == true) {
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
    actionQuyDoi: function () {
        $('.actionQUYDOI').off('click').on('click', function () {
            var id = $(this).data('id');
            console.log('da click' + id);
            $("#ProductValueCountUnit").val(1);
            $("#ProductExchangeUnitResult").val('');
            $.ajax({
                type: "get",
                url: '/admin/exchangeunit/GetExchangeByProductID',
                data: {
                    id:id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        $("#ProductIDExchange").val(data.data.ProductID);
                        $("#ProductIDDisplayExchange").val('SP' + data.data.KyTuCaterory + ChangeIDProduct(data.data.ProductID));
                        $("#ProductNameExchange").val(data.data.ProductName);
                        $("#ProductUnitExchange").val(data.data.UnitName);
                        $("#ProductPriceInputExchange").val(new Intl.NumberFormat().format(data.data.PriceInput));
                        $("#ProductValueUnitExchange").val(data.data.ValueUnit);
                        $("#ProductCountExchange").val(data.data.CountProduct);

                        $("#ProductValueCountUnit").attr({
                            "max": 0,        // substitute your own
                            "min": 1          // values (or variables) here
                        });

                        $("#ProductValueCountUnit").attr({
                            "max": data.data.CountProduct,        // substitute your own
                            "min": 1          // values (or variables) here
                        });

                        $('#ModalExChangeUnit').modal('show')
                    }
                }
            });

            $.ajax({
                type: "get",
                url: '/admin/productunit/GetExchangeByID',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        $("#checkradio").empty();
                        $.each(data.data, function (key, value) {                            
                            $("#checkradio").append('<div class="form-check mb-2"><input class="form-check-input" type ="radio" name="ExchangeUnitRadio" id ="flexRadioDefault" data-value="' + value.ValueUnit + '" data-name="' + value.Name +'"   value="'+value.UnitID+'"><label class="form-check-label" for="ExchangeUnitRadio">' + value.Name + '(Giá trị: ' + value.ValueUnit+')</label></div >');
                        });


                        $('input[type=radio][name=ExchangeUnitRadio]').change(function () {
                            var value = $(this).data('value');
                            var name = $(this).data('name');

                            var id = $(this).val();

                            var valuemuondoi = $("#ProductValueUnitExchange").val();

                            var soluongcandoi = $("#ProductValueCountUnit").val();

                            $("#ProductExchangeUnitResult").val($("#ProductValueCountUnit").val() + " " + $("#ProductUnitExchange").val() + " => " + (valuemuondoi * soluongcandoi) / value + " " + name)
                            $("#ProductExchangeUnitResultCheck").val((valuemuondoi * soluongcandoi) / value);
                            $("#ProductValueCountUnit").change(function () {

                                var soluongcandoimoi = $("#ProductValueCountUnit").val();
                                $("#ProductExchangeUnitResultCheck").val((valuemuondoi * soluongcandoimoi) / value );
                                $("#ProductExchangeUnitResult").val($("#ProductValueCountUnit").val() + " " + $("#ProductUnitExchange").val() + " => " + (valuemuondoi * soluongcandoimoi) / value + " " + name)
                            });
                        });

                    }
                }
            });

            $("#FromAddExchangeUnit").off('submit').on('submit', function (e) {
                e.preventDefault();

                var exchangeUnit = new Object();
                exchangeUnit.ProductID = $("#ProductIDExchange").val();
                exchangeUnit.UnitOut = $("#FromAddExchangeUnit input[type='radio']:checked").val();          
                exchangeUnit.ValueCountUnit = $("#ProductValueCountUnit").val();
                var check = $("#ProductExchangeUnitResultCheck").val();
                if (parseFloat(check) % 1 === 0) {
                    $.ajax({
                        type: "POST",
                        url: '/admin/exchangeunit/AddExchangeUnit',
                        data: exchangeUnit,
                        dataType: 'json',
                        success: function (data) {
                            if (data.code == 200) {
                                if (data.data == true) {
                                    Toast.fire({
                                        icon: 'success',
                                        title: 'Chuyển đổi thành công',
                                    })
                                    $('#ModalExChangeUnit').modal('hide')
                                    ProductController.listProduct()
                                } else {
                                    Toast.fire({
                                        icon: 'error',
                                        title: 'Chuyển đổi thất bại',
                                    })
                                }
                            }
                        }
                    });
                } else {
                    Toast.fire({
                        icon: 'error',
                        title: 'Chuyển đổi thất bại do số lượng đổi ra là số lẻ',
                    })
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
    chooseImageProductDVT: function () {
        $('#ImageProductChooseDVT').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageProductDVT').val(url);
                $("#ImageShowProductDVT").removeAttr('src');
                $("#ImageShowProductDVT").attr('src', url);
            };
            finder.popup();
        });
    },
    chooseImageProductUpdate: function () {
        $('#ImageProductChooseUpdate').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageProductUpdate').val(url);
                $("#ImageShowProductUpdate").removeAttr('src');
                $("#ImageShowProductUpdate").attr('src', url);
                
            };
            finder.popup();
        });
    },
    chooseImageProductDVTUpdate: function () {
        $('#ImageProductChooseDVTUpdate').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageProductDVTUpdate').val(url);
                $("#ImageShowProductDVTUpdate").removeAttr('src');
                $("#ImageShowProductDVTUpdate").attr('src', url);
            };
            finder.popup();
        });
    },

    chooseImageMoreProduct: function () {
        $('#ImageMoreProductChoose').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageList').append('<div class="col-2"><img src="' + url + '" width="100" /><a href="#" class="btndelete" style="margin-left: 42px;"><i class="fas fa-trash" ></i></a>'+'<input type ="hidden" class="hidImage" value ="' + url + '"></div>');

                $('.btndelete').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                })
            };
            finder.popup();
        });
    },
    chooseImageMoreProductDVT: function () {
        $('#ImageMoreProductDVTChoose').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageListDVT').append('<div class="col-2"><img src="' + url + '" width="100" /><a href="#" class="btndeleteDVT" style="margin-left: 42px;"><i class="fas fa-trash" ></i></a>' + '<input type ="hidden" class="hidImageDVT" value ="' + url + '"></div>');

                $('.btndeleteDVT').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                })
            };
            finder.popup();
        });
    },
    chooseImageMoreProductUpdate: function () {
        $('#ImageMoreProductChooseUpdate').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageListUpdate').append('<div class="col-2"><img src="' + url + '" width="100" /><a href="#" class="btnupdatedelete" style="margin-left: 42px;"><i class="fas fa-trash" ></i></a>' + '<input type ="hidden" class="hidImage" value ="' + url + '"></div>');

                $('.btnupdatedelete').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                })
            };
            finder.popup();
        });
    },
    chooseImageMoreProductDVTUpdate: function () {
        $('#ImageMoreProductDVTChooseUpdate').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageListDVTUpdate').append('<div class="col-2"><img src="' + url + '" width="100" /><a href="#" class="btnupdatedeleteDVT" style="margin-left: 42px;"><i class="fas fa-trash" ></i></a>' + '<input type ="hidden" class="hidImageDVTUpdate" value ="' + url + '"></div>');

                $('.btnupdatedeleteDVT').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                })
            };
            finder.popup();
        });
    },
    searchNangCap: function () {
        $("#btnSearchNangCao").off('click').on('click', function () {
            $('#SearchNangCao').toggle();;


        })

    },
    addProduct: function () {
        ProductController.chooseImageProduct();
        ProductController.chooseImageMoreProduct();
        $('#CreateSanPham').off('click').on('click', function () {
            $('#ModalCreateSP').modal('show');
            ProductController.listProductCategory()
            ProductController.listProductUnitBase()
            CKEDITOR.replace('DiscriptionProduct', {
                customConfig: '/Assets/Admin/plugins/ckeditor/config.js'
            });
            $("#FromCreateProduct").off('submit').on('submit', function (e) {

                e.preventDefault();

                var imagemore = [];

                $.each($('#ImageList .hidImage'), function (i, item) {
                    imagemore.push($(item).val());
                });


                var product = new Object();
                var txt = CKEDITOR.instances['DiscriptionProduct'].getData();
                product.Name = $("#NameProduct").val();
                product.PriceOut = $("#PriceOutProduct").val().split('.').join("");
                product.Pricewholesale = $("#PricewholesaleProduct").val().split('.').join("");
                product.Image = $("#ImageProduct").val();
                product.MoreImage = JSON.stringify(imagemore);
                product.UnitID = $("#UnitProduct").val();
                product.ProductCategoryID = $("#productcategory").val();
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
                    url: '/admin/product/AddProduct',
                    data: product,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công!.',
                                })
                                $('#paginationproduct').twbsPagination('destroy');
                                $("#NameProduct").val('');
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


                                ProductController.listProduct()
                                ProductController.listProductDVT()
                                $('#ModalCreateSP').modal('hide');
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
    addProductDVT: function () {
        ProductController.listProductUnit()
        ProductController.listProductDVT()
        ProductController.chooseImageProductDVT()
        ProductController.chooseImageMoreProductDVT()
        $('#CreateSanPhamDVT').off('click').on('click', function () {
            $('#ModalCreateSPDVT').modal('show')
            
            $("#FromCreateProductUnit").off('submit').on('submit', function (e) {

                e.preventDefault();

                var imagemore = [];

                $.each($('#ImageListDVT .hidImageDVT'), function (i, item) {
                    imagemore.push($(item).val());
                });

                e.preventDefault();
                var product = new Object();
                product.ProductID = $("#ProductDVT").val();
                product.Name = $("#NameProductDVT").val();
                product.PriceOut = $("#PriceOutProductDVT").val().split('.').join("");
                product.Pricewholesale = $("#PricewholesaleProductDVT").val().split('.').join("");
                product.UnitID = $("#UnitProductDVT").val();
                product.Image = $("#ImageProductDVT").val();
                product.MoreImage = JSON.stringify(imagemore);
                $.ajax({
                    type: "POST",
                    url: '/admin/product/AddProductDVT',
                    data: product,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công',
                                })

                                $("#NameProductDVT").val('');
                                $("#PriceOutProductDVT").val('')
                                $("#PricewholesaleProductDVT").val('')
                                $("#ImageProductDVT").val('')
                                $("#ImageShowProductDVT").removeAttr('src');


                                $('#paginationproduct').twbsPagination('destroy');
                                ProductController.listProduct()
                                ProductController.listProductUnit()                               
                                $("#ImageShowProductDVT").removeAttr('src');
                                $('#ModalCreateSPDVT').modal('hide')
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
    deleteProduct: function () {
        $(".actiondeleteProduct").off('click').on('click', function () {

            var id = $(this).data('id');

            var name = $(this).data('name');

            $("#NameProductDelete").empty();
            $("#NameProductDelete").append('Bạn có chắc chắn xóa: ' + name);

            $("#ResultProductDelete").attr("hidden", true);

            $("#ModalDeleteProduct").modal('show');

            $("#btndeleteProduct").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/product/DeleteProduct',
                    type: 'get',
                    data: {
                        id: id
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code = 200) {
                            if (data.data.Check == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Xóa thành công',
                                })
                                $('#ModalDeleteProduct').modal('hide')
                                ProductController.listProduct();

                            } else {
                                $("#ResultProductDelete").empty();
                                $("#ResultProductDelete").removeAttr("hidden");
                                $("#ResultProductDelete").append(data.data.Result);
                            }
                        }
                    }
                });
            })
        })
    },
    updateProduct: function () {

        ProductController.chooseImageProductDVTUpdate();
        ProductController.chooseImageProductUpdate();
        ProductController.chooseImageMoreProductUpdate();
        ProductController.chooseImageMoreProductDVTUpdate();
        CKEDITOR.replace('DiscriptionProductUpdate', {
            customConfig: '/Assets/Admin/plugins/ckeditor/config.js'
        });
        $('.modalshowEdit').off('click').on('click', function () {

            var id = $(this).data('id');
            var unit = $(this).data('unitid');
            
            if (unit == '0') {
                
                $.ajax({
                    type: "GET",
                    url: '/admin/product/DetailProduct',
                    data: {
                        id: id
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            $("#IDProductUpdate").val(data.data.ProductID);
                            $("#NameProductUpdate").val(data.data.Name);
                            $("#PriceOutProductUpdate").val(new Intl.NumberFormat().format(data.data.PriceOut));
                            $("#PricewholesaleProductUpdate").val(new Intl.NumberFormat().format(data.data.Pricewholesale));
                            $("#UnitProductUpdate").val(data.data.UnitID).change();
                            $("#ImageProductUpdate").val(data.data.Image);
                            $("#ImageShowProductUpdate").removeAttr('src');
                            $("#ImageShowProductUpdate").attr('src', data.data.Image);

                            var ImageMore = data.images;
                            var html = '';
                            $.each(ImageMore, function (i, item) {

                                html += '<div class="col-2"><img src="' + item + '" width="100" /><a href="#" class="btnupdatedelete" style="margin-left: 42px;"><i class="fas fa-trash" ></i></a>' + '<input type ="hidden" class="hidImage" value ="' + item + '"></div>';
                                
                            })
                            $('#ImageListUpdate').html(html);

                            $('.btnupdatedelete').off('click').on('click', function (e) {
                                e.preventDefault();
                                $(this).parent().remove();
                            })

                            $(`#productcategoryUpdate option[value='${data.data.ProductCategoryID}']`).prop('selected', true);
                            if (data.data.Discription != null || data.data.Discription!='') {
                                CKEDITOR.instances['DiscriptionProductUpdate'].setData(data.data.Discription);
                            }                           
                            $("#TradeMarkProductUpdate").val(data.data.TradeMark);
                            $("#TradeOriginProductUpdate").val(data.data.TradeOrigin);
                            $("#IngredientProductUpdate").val(data.data.Ingredient);
                            $("#ProductionProductUpdate").val(data.data.Production);
                            $("#ExpiryProductUpdate").val(data.data.Expiry);
                            $("#UserManualProductUpdate").val(data.data.UserManual);
                            $("#CareInstructionsProductUpdate").val(data.data.CareInstructions);
                            $("#PackingProductUpdate").val(data.data.Packing);
                        }
                    }
                });
                            
                ProductController.listPricePromotion(id);
                $('#ModalUpdateSP').modal('show');
                $("#FromUpdateProduct").off('submit').on('submit', function (e) {


                    var imagemore = [];

                    $.each($('#ImageListUpdate .hidImage'), function (i, item) {
                        imagemore.push($(item).val());
                    });


                    e.preventDefault();
                    var product = new Object();
                    var txt = CKEDITOR.instances['DiscriptionProductUpdate'].getData();
                    if (txt != null) {
                        console.log('khong co null')
                    }
                    product.ProductID = id;
                    product.Name = $("#NameProductUpdate").val();
                    product.PriceOut = $("#PriceOutProductUpdate").val().split('.').join("");
                    product.Pricewholesale = $("#PricewholesaleProductUpdate").val().split('.').join("");
                    product.UnitID = $("#UnitProductUpdate").val();
                    product.ProductCategoryID = $("#productcategoryUpdate").val();
                    product.Image = $("#ImageProductUpdate").val();
                    product.MoreImage = JSON.stringify(imagemore);
                    product.Discription = txt;
                    product.TradeMark = $("#TradeMarkProductUpdate").val();
                    product.TradeOrigin = $("#TradeOriginProductUpdate").val();
                    product.Ingredient = $("#IngredientProductUpdate").val();
                    product.Production = $("#ProductionProductUpdate").val();
                    product.Expiry = $("#ExpiryProductUpdate").val();
                    product.UserManual = $("#UserManualProductUpdate").val();
                    product.CareInstructions = $("#CareInstructionsProductUpdate").val();
                    product.Packing = $("#PackingProductUpdate").val();
                    $.ajax({
                        type: "POST",
                        url: '/admin/product/UpdateProduct',
                        data: product,
                        dataType: 'json',
                        success: function (data) {
                            if (data.code == 200) {
                                if (data.data == true) {
                                    $('#ModalUpdateSP').modal('hide');
                                    Toast.fire({
                                        icon: 'success',
                                        title: 'cập nhật thành công!.',
                                    })
                                    ProductController.listProduct();
                                } else {
                                    Toast.fire({
                                        icon: 'success',
                                        title: 'Cập nhật thất bại',
                                    })
                                }
                            }
                        }
                    });


                });

                ProductController.createpricepromotion(id, 0);
                $('#paginationpromotion').twbsPagination('destroy');
            }
            else {
                $.ajax({
                    type: "GET",
                    url: '/admin/product/DetailProduct',
                    data: {
                        id: id
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            $("#IDProductDVTUpdate").val(data.data.ProductID);
                            $("#NameProductDVTUpdate").val(data.data.Name);
                            $("#PriceOutProductDVTUpdate").val(new Intl.NumberFormat().format(data.data.PriceOut));
                            $("#PricewholesaleProductDVTUpdate").val(new Intl.NumberFormat().format(data.data.Pricewholesale));
                            $("#UnitProductDVTUpdate").val(data.data.UnitID).change();
                            $("#ImageProductDVTUpdate").val(data.data.Image);
                            $("#ImageShowProductDVTUpdate").removeAttr('src');
                            $("#ImageShowProductDVTUpdate").attr('src', data.data.Image);

                            var ImageMore = data.images;
                            var html = '';
                            $.each(ImageMore, function (i, item) {

                                html += '<div class="col-2"><img src="' + item + '" width="100" /><a href="#" class="btnupdatedeleteDVT" style="margin-left: 42px;"><i class="fas fa-trash" ></i></a>' + '<input type ="hidden" class="hidImageDVTUpdate" value ="' + item + '"></div>';

                            })
                            $('#ImageListDVTUpdate').html(html);

                            $('.btnupdatedeleteDVT').off('click').on('click', function (e) {
                                e.preventDefault();
                                $(this).parent().remove();
                            })
                            
                        }
                    }
                });

                ProductController.listPricePromotionDVT(id);
                $('#ModalUpdatePDVT').modal('show');
                $("#FromUpdateProductUnit").off('submit').on('submit', function (e) {


                    var imagemore = [];

                    $.each($('#ImageListDVTUpdate .hidImageDVTUpdate'), function (i, item) {
                        imagemore.push($(item).val());
                    });


                    e.preventDefault();
                    var product = new Object();
                    product.ProductID = id;
                    product.Name = $("#NameProductDVTUpdate").val();
                    product.PriceOut = parseFloat($("#PriceOutProductDVTUpdate").val().split('.').join(""));
                    product.Pricewholesale = parseFloat($("#PricewholesaleProductDVTUpdate").val().split('.').join(""));
                    product.UnitID = $("#UnitProductDVTUpdate").val();
                    product.Image = $("#ImageProductDVTUpdate").val();
                    product.MoreImage = JSON.stringify(imagemore);
                    $.ajax({
                        type: "POST",
                        url: '/admin/product/UpdateProductDVT',
                        data: product,
                        dataType: 'json',
                        success: function (data) {
                            if (data.code == 200) {
                                if (data.data == true) {
                                    $('#ModalUpdatePDVT').modal('hide');
                                    Toast.fire({
                                        icon: 'success',
                                        title: 'cập nhật thành công!.',
                                    })
                                    ProductController.listProduct();
                                } else {
                                    Toast.fire({
                                        icon: 'success',
                                        title: 'Cập nhật thất bại',
                                    })
                                }
                            }
                        }
                    });


                });

                ProductController.createpricepromotionDVT(id, 0);
                $('#paginationpromotionDVT').twbsPagination('destroy');
            }
            
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    listProduct: function (changePageSize) {

        var search = new Object();
        search.querysearch = $("#search-querysearch").val();
        search.TinhTrang = $("#search-TinhTrang").val();
        search.ProductCategoryID = $("#search-productcategory").val();
        search.SoLuong = $("#search-SoLuong").val();
        search.UnitID = $("#search-productUnit").val();
        search.GiaLeTu = $("#GiaLeTu").val().split('.').join("");
        search.GiaLeDen = $("#GiaLeDen").val().split('.').join("");
        search.GiaSiTu = $("#GiaSiTu").val().split('.').join("");
        search.GiaSiDen = $("#GiaSiDen").val().split('.').join("");
        $.ajax({
            url: '/admin/product/ListProduct',
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
                            IconQuyDoi: QuyDoiDVT(value.CountProduct,value.ParentProductID)
                        });
                    });
                    $('#tbProduct').html(html);

                    $('#TongKho').empty();
                    $('#TongKho').append(data.TongKho);

                    if (data.total != 0) {
                        ProductController.paging(data.total, function () {
                            ProductController.listProduct();
                        }, changePageSize)
                    }
                    

                    ProductController.actionDisplayProduct();
                    ProductController.actionQuyDoi();

                    ProductController.listProductCategoryUpdate();
                    ProductController.listProductUnitBaseUpdate();
                    ProductController.listProductUnitUpdate();
                    ProductController.updateProduct();
                    ProductController.deleteProduct();
                }
            }
        });
        function QuyDoiDVT(count,dvt) {
            if (count!=0&&dvt!='0') {
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
    listProductCategory: function () {
        $.ajax({
            url: '/admin/productcategory/ListProductCategory',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#productcategory');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.ProductCategoryID).text(val.Name).appendTo($select);
                    });
                }
            }
        });
    },
    listProductUnitBase: function () {
        $.ajax({
            url: '/admin/productunit/ListProductUnitBase',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#UnitProduct');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.UnitID).text(val.Name).appendTo($select);
                    });
                }
            }
        });

    },
    listProductCategoryUpdate: function () {
        $.ajax({
            url: '/admin/productcategory/ListProductCategory',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#productcategoryUpdate');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.ProductCategoryID).text(val.Name).appendTo($select);
                    });
                }
            }
        });
    },
    listProductUnitBaseUpdate: function () {
        $.ajax({
            url: '/admin/productunit/ListProductUnitBase',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#UnitProductUpdate');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.UnitID).text(val.Name).appendTo($select);
                    });
                }
            }
        });

    },
    listProductDVT: function () {
        $.ajax({
            url: '/admin/product/ListProductDVTCoBan',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#ProductDVT');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.ProductID).text(val.Name + ' - ĐVT: ' + val.UnitName).appendTo($select);
                    });
                }
            }
        });
    },
    listProductUnit: function () {       
        $.ajax({
            url: '/admin/productunit/ListProductUnit',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#UnitProductDVT');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.UnitID).text(val.Name + ' - 1 ' + val.Name +' = ' + val.ValueUnit).appendTo($select);
                    });


                }
            }
        });

    },
    listProductUnitUpdate: function () {
        $.ajax({
            url: '/admin/productunit/ListProductUnit',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#UnitProductDVTUpdate');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.UnitID).text(val.Name + ' - 1 ' + val.Name + ' = ' + val.ValueUnit).appendTo($select);
                    });


                }
            }
        });

    },
    listPricePromotion: function (id, changePageSize) {
        
        $.ajax({
            type: "GET",
            url: '/admin/productpromotion/ListPricePromotion',
            data: {
                id: id,
                page: pgpromotion.pageIndex,
                pageSize: pgpromotion.pageSize
            },
            dataType: 'json',
            success: function (data) {
                if (data.code == 200) {
                    var html = "";
                    var template = $('#data-templateProductPromotion').html();
                    var dem = 1;
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.PricePromotionID,
                            ProductID: value.ProductID,
                            CreateDate: parseJsonDate(value.CreateDate),
                            PricePromotion: new Intl.NumberFormat().format(value.PricePromotion),
                            PricewholesalePromotion: new Intl.NumberFormat().format(value.PricewholesalePromotion),
                            StartDate: parseJsonDate(value.StartDate),
                            EndDate: parseJsonDate(value.EndDate),
                            PromotionStatus: value.PromotionStatus == true ? "style=\"background-color:#25BBF7; color: white\"" : ""

                        });
                        dem++;
                    });
                    $('#tbProductPromotion').html(html);

                    if (data.total != 0) {
                        ProductController.pagingPromotion(data.total, function () {
                            ProductController.listPricePromotion(id);
                        }, changePageSize)
                    }
                    
                    ProductController.updatePricePromotion();
                }
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    createproduct: function () {

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
                                ProductController.listAccount(true);
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
    createpricepromotion: function (idproduct,idpricepromotion) {
        $("#FormQLGiamGia").off('submit').on('submit', function (e) {
            e.preventDefault();
            var pricepromotion = new Object();
            pricepromotion.PricePromotionID = idpricepromotion;
            pricepromotion.ProductID = idproduct;
            pricepromotion.PricePromotion = $("#PricePromotion").val();
            pricepromotion.PricewholesalePromotion = $("#PricewholesalePromotion").val();
            pricepromotion.StartDate = $("#StartDate").val();
            pricepromotion.EndDate = $("#EndDate").val();
            $.ajax({
                type: "POST",
                url: '/admin/productpromotion/CreatePricePromotion',
                data: pricepromotion,
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data !='') {
                            Toast.fire({
                                icon: 'success',
                                title: 'Thêm giảm giá thành công',
                            })
                            ProductController.listPricePromotion(data.data);
                            $("#PricePromotion").val('');
                            $("#PricewholesalePromotion").val('');
                            $("#StartDate").val('');
                            $("#EndDate").val('');
                        } else {
                            Toast.fire({
                                icon: 'error',
                                title: 'không thể thêm do ngày bắt đâu nhỏ hơn ngày kết thúc đã có trong CSDL - trong hàm create',
                            })

                        }
                    }
                }
            });
        });
    },
    updatePricePromotion: function () {
        $('.EditPromotion').off('click').on('click', function (e) {
            $('#UpdatePromotion').removeAttr('disabled');
            $('#CreatePromotion').attr('disabled', 'disabled');
            $('#StartDate').attr('disabled', 'disabled');
            $('#EndDate').attr('disabled', 'disabled');
            var idpromotion = $(this).data('idprice');

            console.log(idpromotion);
            $.ajax({
                type: "GET",
                url: '/admin/productpromotion/Detail',
                data: {
                    id: idpromotion
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {                       
                        $("#PricePromotion").val(data.data.PricePromotion);
                        $("#PricewholesalePromotion").val(data.data.PricewholesalePromotion);
                        $("#StartDate").val(parseJsonDate(data.data.StartDate));
                        $("#EndDate").val(parseJsonDate(data.data.EndDate));
                    }
                }
            });
            $("#UpdatePromotion").off('click').on('click', function (e) {
                e.preventDefault();
                $('#UpdatePromotion').attr('disabled', 'disabled');
                $('#CreatePromotion').removeAttr('disabled');
                $('#StartDate').removeAttr('disabled');
                $('#EndDate').removeAttr('disabled');
                e.preventDefault();
                var pricepromotion = new Object();
                pricepromotion.PricePromotionID = idpromotion;
                pricepromotion.ProductID = 0;
                pricepromotion.PricePromotion = $("#PricePromotion").val();
                pricepromotion.PricewholesalePromotion = $("#PricewholesalePromotion").val();
                pricepromotion.StartDate = $("#StartDate").val();
                pricepromotion.EndDate = $("#EndDate").val();
                $.ajax({
                    type: "POST",
                    url: '/admin/productpromotion/CreatePricePromotion',
                    data: pricepromotion,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data != ''&&data.data!=null) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'cập nhật giảm giá thành công',
                                })
                                ProductController.listPricePromotion(data.data);
                                $("#PricePromotion").val('');
                                $("#PricewholesalePromotion").val('');
                                $("#StartDate").val('');
                                $("#EndDate").val('');
                            } else if (data.data == null) {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'không thể cập nhật do không điền giá giảm',
                                })
                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'không thể cập nhật',
                                })
                            }
                        }
                    }
                });
            });
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));
         
            return `${dt.getFullYear().toString().padStart(4, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}T${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0')}`
        }
    },
    listPricePromotionDVT: function (id,changePageSize) {
        $.ajax({
            type: "GET",
            url: '/admin/productpromotion/ListPricePromotion',
            data: {
                id: id,
                page: pgpromotionDVT.pageIndex,
                pageSize: pgpromotionDVT.pageSize
            },
            dataType: 'json',
            success: function (data) {
                if (data.code == 200) {
                    var html = "";
                    var template = $('#data-templateProductPromotionDVT').html();
                    var dem = 1;
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.PricePromotionID,
                            ProductID: value.ProductID,
                            CreateDate: parseJsonDate(value.CreateDate),
                            PricePromotion: new Intl.NumberFormat().format(value.PricePromotion),
                            PricewholesalePromotion: new Intl.NumberFormat().format(value.PricewholesalePromotion),
                            StartDate: parseJsonDate(value.StartDate),
                            EndDate: parseJsonDate(value.EndDate),
                            PromotionStatus: value.PromotionStatus == true ? "style=\"background-color:#25BBF7; color: white\"" : ""


                        });
                        dem++;
                    });
                    $('#tbProductPromotionDVT').html(html);
                    if (data.total != 0) {
                        ProductController.pagingPromotionDVT(data.total, function () {
                            ProductController.listPricePromotionDVT(id);
                        }, changePageSize)
                    }
                    ProductController.updatePricePromotionDVT();
                }
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    createpricepromotionDVT: function (idproduct, idpricepromotion) {
        $("#FormQLGiamGiaDVT").off('submit').on('submit',  function (e) {
            e.preventDefault();
            var pricepromotion = new Object();
            pricepromotion.PricePromotionID = idpricepromotion;
            pricepromotion.ProductID = idproduct;
            pricepromotion.PricePromotion = $("#PricePromotionDVT").val();
            pricepromotion.PricewholesalePromotion = $("#PricewholesalePromotionDVT").val();
            pricepromotion.StartDate = $("#StartDateDVT").val();
            pricepromotion.EndDate = $("#EndDateDVT").val();
            $.ajax({
                type: "POST",
                url: '/admin/productpromotion/CreatePricePromotion',
                data: pricepromotion,
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data != '') {
                            Toast.fire({
                                icon: 'success',
                                title: 'Thêm giảm giá thành công',
                            })
                            ProductController.listPricePromotionDVT(data.data);
                            $("#PricePromotionDVT").val('');
                            $("#PricewholesalePromotionDVT").val('');
                            $("#StartDateDVT").val('');
                            $("#EndDateDVT").val('');
                        } else {
                            Toast.fire({
                                icon: 'error',
                                title: 'không thể thêm do ngày bắt đâu nhỏ hơn ngày kết thúc đã có trong CSDL - trong hàm create',
                            })
                        }
                    }
                }
            });
        });
    },
    updatePricePromotionDVT: function () {
        $('.EditPromotionDVT').off('click').on('click', function (e) {
            $('#UpdatePromotionDVT').removeAttr('disabled');
            $('#CreatePromotionDVT').attr('disabled', 'disabled');
            $('#StartDateDVT').attr('disabled', 'disabled');
            $('#EndDateDVT').attr('disabled', 'disabled');
            var idpromotion = $(this).data('idprice');

            console.log(idpromotion);
            $.ajax({
                type: "GET",
                url: '/admin/productpromotion/Detail',
                data: {
                    id: idpromotion
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        $("#PricePromotionDVT").val(data.data.PricePromotion);
                        $("#PricewholesalePromotionDVT").val(data.data.PricewholesalePromotion);
                        $("#StartDateDVT").val(parseJsonDate(data.data.StartDate));
                        $("#EndDateDVT").val(parseJsonDate(data.data.EndDate));
                    }
                }
            });
            $("#UpdatePromotionDVT").off('click').on('click', function (e) {
                e.preventDefault();
                $('#UpdatePromotionDVT').attr('disabled', 'disabled');
                $('#CreatePromotionDVT').removeAttr('disabled');
                $('#StartDateDVT').removeAttr('disabled');
                $('#EndDateDVT').removeAttr('disabled');
                e.preventDefault();
                var pricepromotion = new Object();
                pricepromotion.PricePromotionID = idpromotion;
                pricepromotion.ProductID = 0;
                pricepromotion.PricePromotion = $("#PricePromotionDVT").val();
                pricepromotion.PricewholesalePromotion = $("#PricewholesalePromotionDVT").val();
                pricepromotion.StartDate = $("#StartDateDVT").val();
                pricepromotion.EndDate = $("#EndDateDVT").val();
                $.ajax({
                    type: "POST",
                    url: '/admin/productpromotion/CreatePricePromotion',
                    data: pricepromotion,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data != '' && data.data != 'NULL') {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'cập nhật giảm giá thành công',
                                })
                                ProductController.listPricePromotionDVT(data.data);
                                $("#PricePromotionDVT").val('');
                                $("#PricewholesalePromotionDVT").val('');
                                $("#StartDateDVT").val('');
                                $("#EndDateDVT").val('');
                            } else if (data.data == 'NULL') {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'không thể cập nhật do không điền giá giảm',
                                })
                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'không thể cập nhật',
                                })
                            }
                        }
                    }
                });
            });
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getFullYear().toString().padStart(4, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}T${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0')}`
        }
    },
    SearchLstSelecttCategory: function () {

        $.ajax({
            url: '/admin/productcategory/ListProductCategory',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#search-productcategory');
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.ProductCategoryID).text(val.Name).appendTo($select);
                    });
                }
            }
        });
        $.ajax({
            url: '/admin/productunit/ListProductUnitselectSearch',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#search-productUnit');
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.UnitID).text(val.Name + ' - giá trị: ' + val.ValueUnit).appendTo($select);
                    });
                }
            }
        });

    },
    SearchProduct: function () {

        $("#BtnSearchProduct").off('click').on('click', function () {
            $('#paginationproduct').twbsPagination('destroy');
            ProductController.listProduct();
        })
    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
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
                pg.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    pagingPromotion: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#paginationpromotion a').length === 0 || changePageSize === true) {
            $('#paginationpromotion').empty();
            $('#paginationpromotion').removeData('twbs-pagination');
            $('#paginationpromotion').unbind("page");
        }
        $('#paginationpromotion').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgpromotion.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    pagingPromotionDVT: function (totalRow, callback, changePageSize) {
        
        var totalPage = Math.ceil(totalRow / pg.pageSize)

        if ($('#paginationpromotionDVT a').length === 0 || changePageSize === true) {
            $('#paginationpromotionDVT').empty();
            $('#paginationpromotionDVT').removeData('twbs-pagination');
            $('#paginationpromotionDVT').unbind("page");
        }
        
        $('#paginationpromotionDVT').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgpromotionDVT.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
}
ProductController.init();