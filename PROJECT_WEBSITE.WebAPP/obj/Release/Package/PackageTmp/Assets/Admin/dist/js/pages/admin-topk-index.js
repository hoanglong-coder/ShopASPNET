
var pgcategory = {
    pageSize: 10,
    pageIndex: 1
}
var pgcategory2 = {
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
var TKUController = {
    init: function () {
        //Khởi tạo theme
        TKUController.initTheme();
        TKUController.CreateTxt();
        TKUController.ActionTopk();
        TKUController.readFiletxt();

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    CreateTxt: function () {
        $("#CreateTxt").off('click').on('click', function () {
            $(".spinner-border").show();
            $.ajax({
                type: "get",
                url: '/admin/tkualgo/CreateTXT',
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        console.log('vao roi')
                        if (data.data == true) {
                            $(".spinner-border").hide();

                            Toast.fire({
                                icon: 'success',
                                title: 'Thành công',
                            })

                            var url = "Data/"+data.NameFile;

                            $.ajax({
                                url: url,
                                cache: false,
                                xhr: function () {
                                    var xhr = new XMLHttpRequest();
                                    xhr.onreadystatechange = function () {
                                        if (xhr.readyState == 2) {
                                            if (xhr.status == 200) {
                                                xhr.responseType = "blob";
                                            } else {
                                                xhr.responseType = "text";
                                            }
                                        }
                                    };
                                    return xhr;
                                },
                                success: function (data) {
                                    //Convert the Byte Data to BLOB object.
                                    var blob = new Blob([data], { type: "application/octetstream" });

                                    //Check the Browser type and download the File.
                                    var isIE = false || !!document.documentMode;
                                    if (isIE) {
                                        window.navigator.msSaveBlob(blob, fileName);
                                    } else {
                                        var url = window.URL || window.webkitURL;
                                        link = url.createObjectURL(blob);
                                        var a = $("<a />");
                                        a.attr("download", fileName);
                                        a.attr("href", link);
                                        $("body").append(a);
                                        a[0].click();
                                        $("body").remove(a);
                                    }
                                }
                            });

                        } else {
                            $(".spinner-border").hide();
                            Toast.fire({
                                icon: 'success',
                                title: 'Thất bại',
                            })
                        }
                    }
                }
            });

        })
    },
    ActionTopk: function () {
        $("#actiontopk").off('click').on('click', function () {

            $.ajax({
                type: "POST",
                url: '/admin/tkualgo/StartTKU',
                data: {
                    path: $("#filepath").val(),
                    topk: $("#TOPK").val()
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data == true) {
                            Toast.fire({
                                icon: 'success',
                                title: 'Thành công',
                            })


                        } else {                         
                            Toast.fire({
                                icon: 'error',
                                title: 'Thất bại',
                            })
                        }
                    }
                }
            });


        })
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


                            TKUController.DetailProduct(data.data);

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
                            id:id
                        },
                        dataType: 'json',
                        success: function (data) {
                            if (data.code == 200) {
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
    }
}
TKUController.init();