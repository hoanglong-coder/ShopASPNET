var pg = {
    pageSize: 10,
    pageIndex: 1
}

var pgcategory = {
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
var DiscountCodeController = {
    init: function () {
        //Khởi tạo theme
        DiscountCodeController.initTheme();

        DiscountCodeController.lstdiscount();

        DiscountCodeController.CreateDiscount();

        DiscountCodeController.searchdiscount();


    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    searchdiscount: function () {

        $("#btnTiemKiem").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationdiscount').twbsPagination('destroy');
            DiscountCodeController.lstdiscount();

        })
    },
    lstdiscount: function (changePageSize) {
        var search = new Object();
        search.SearchName = $("#search-discount").val();
        search.TuNgay = $("#TuNgay").val();
        search.DenNgay = $("#DenNgay").val();

        $.ajax({
            url: '/admin/DiscountCode/ListDiscountCode',
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
                    var template = $('#data-templatediscount').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            DiscountCodeID: value.DiscountCodeID,
                            Name: value.Name,
                            StartDate: parseJsonDate(value.StartDate),
                            EndDate: parseJsonDate(value.EndDate),
                            CreateDate: parseJsonDate(value.CreateDate),
                            PercentCart: value.PercentCart,
                            TotalCart: new Intl.NumberFormat().format(value.TotalCart),
                            DistcountCount: value.DistcountCount,
                            DiscountStatus: Status(value.DiscountStatus)
                        });
                    });
                    $('#tbNews').html(html);
                    if (data.total != 0) {
                        DiscountCodeController.paging(data.total, function () {
                            DiscountCodeController.lstdiscount();
                        }, changePageSize)
                    }
                }

                DiscountCodeController.editdiscount();
                DiscountCodeController.deletediscount();
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
        function Status(value) {
            if (value == true) {
                return "<span class=\"badge bg-primary\">Đang hoạt động</span>";
            }
            if (value == false) {
                return "<span class=\"badge bg-danger\">Dừng</span>";
            }
        }

    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#paginationdiscount a').length === 0 || changePageSize === true) {
            $('#paginationdiscount').empty();
            $('#paginationdiscount').removeData('twbs-pagination');
            $('#paginationdiscount').unbind("page");
        }
        $('#paginationdiscount').twbsPagination({
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
    CreateDiscount: function () {
        $("#CreateDiscount").off('click').on('click', function () {

            var percen = $('input[name = "flexRadioDefault"]:checked').val();

            if (percen == 0) {
                $('#Total').attr('disabled', 'disabled');

                $('#Total').val('');
                $('#Percent').removeAttr('disabled');

            } else {
               
                $('#Total').removeAttr('disabled');

                $('#Percent').val('');
                $('#Percent').attr('disabled', 'disabled');
            }

            DiscountCodeController.ChangePhuongThuc();

            $("#ModalCreateDiscount").modal('show');

            $("#FromCreateDiscount").off('submit').on('submit', function (e) {
                e.preventDefault();
                var percen2 = $('input[name = "flexRadioDefault"]:checked').val();
                if (percen2 == 0) {
                    if ($("#Percent").val() == "") {
                        Toast.fire({
                            icon: 'error',
                            title: 'Chưa nhập phần trăm',
                        })
                    } else {
                        var discount = new Object();
                        discount.Name = $("#NameDiscount").val();
                        discount.StartDate = $("#StartDateDiscount").val();
                        discount.EndDate = $("#EndDateDiscount").val();
                        discount.PercentCart = $("#Percent").val();
                        discount.TotalCart = $("#Total").val().split('.').join("");
                        discount.DistcountCount = $("#CountDiscount").val();


                        $.ajax({
                            type: "post",
                            url: '/admin/DiscountCode/CreateDiscountCode',
                            data: discount,
                            dataType: 'json',
                            success: function (data) {
                                if (data.code == 200) {
                                    if (data.data.Check == true) {
                                        $("#ModalCreateDiscount").modal('hide');
                                        $("#NameDiscount").val('');
                                        $("#StartDateDiscount").val('');
                                        $("#EndDateDiscount").val('');
                                        $("#Percent").val('');
                                        $("#Total").val('');
                                        $("#CountDiscount").val('');
                                        Toast.fire({
                                            icon: 'success',
                                            title: 'Thành công',
                                        })
                                        DiscountCodeController.lstdiscount();
                                        $('#paginationdiscount').twbsPagination('destroy');

                                    } else {
                                        Toast.fire({
                                            icon: 'error',
                                            title: data.data.Result,
                                        })
                                    }
                                }
                            }
                        });
                    }
                } else {
                    if ($("#Total").val() == "") {
                        Toast.fire({
                            icon: 'error',
                            title: 'Chưa nhập tiền giảm',
                        })
                    } else {
                        var discount = new Object();
                        discount.Name = $("#NameDiscount").val();
                        discount.StartDate = $("#StartDateDiscount").val();
                        discount.EndDate = $("#EndDateDiscount").val();
                        discount.PercentCart = $("#Percent").val();
                        discount.TotalCart = $("#Total").val().split('.').join("");
                        discount.DistcountCount = $("#CountDiscount").val();


                        $.ajax({
                            type: "post",
                            url: '/admin/DiscountCode/CreateDiscountCode',
                            data: discount,
                            dataType: 'json',
                            success: function (data) {
                                if (data.code == 200) {
                                    if (data.data.Check == true) {
                                        $("#ModalCreateDiscount").modal('hide');
                                        $("#NameDiscount").val('');
                                        $("#StartDateDiscount").val('');
                                        $("#EndDateDiscount").val('');
                                        $("#Percent").val('');
                                        $("#Total").val('');
                                        $("#CountDiscount").val('');
                                        Toast.fire({
                                            icon: 'success',
                                            title: 'Thành công',
                                        })
                                        DiscountCodeController.lstdiscount();
                                        $('#paginationdiscount').twbsPagination('destroy');

                                    } else {
                                        Toast.fire({
                                            icon: 'error',
                                            title: data.data.Result,
                                        })
                                    }
                                }
                            }
                        });
                    }
                }

                

            })
        })
    },
    ChangePhuongThuc: function () {
        $('input[type=radio][name=flexRadioDefault]').change(function () {
            if (this.value == 0) {
                $('#Total').attr('disabled', 'disabled');

                $('#Total').val('');
                $('#Percent').removeAttr('disabled');
            }
            else if (this.value == 1) {
                
                $('#Total').removeAttr('disabled');
                $('#Percent').val('');
                $('#Percent').attr('disabled', 'disabled');
            }
        });
    },
    editdiscount: function () {

        $(".modalshowEdit").off('click').on('click', function () {

            var id = $(this).data('id');

            $.ajax({
                type: "get",
                url: '/admin/DiscountCode/GetByID',
                data: {
                    id:id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        console.log(data.data)
                        $("#IDDiscountedit").val(data.data.DiscountCodeID);
                        $("#NameDiscountedit").val(data.data.Name);
                        $("#StartDateDiscountedit").val(parseJsonDate(data.data.StartDate));
                        $("#EndDateDiscountedit").val(parseJsonDate(data.data.EndDate));
                        $("#Percentedit").val(data.data.PercentCart);
                        $("#Totaledit").val(new Intl.NumberFormat().format(data.data.TotalCart));
                        $("#CountDiscountedit").val(data.data.DistcountCount);
                        console.log(parseJsonDate(data.data.StartDate));
                        if (data.data.PercentCart != null) {
                            $('input:radio[name="flexRadioDefaultedit"][value="0"]').prop('checked', true);
                        }
                        if (data.data.TotalCart != null) {
                            $('input:radio[name="flexRadioDefaultedit"][value="1"]').prop('checked', true);
                        }
                            
                        var percen = $('input[name = "flexRadioDefaultedit"]:checked').val();

                        if (percen == 0) {
                            $('#Totaledit').attr('disabled', 'disabled');

                            $('#Percentedit').removeAttr('disabled');

                        } else {

                            $('#Totaledit').removeAttr('disabled');

                            $('#Percentedit').attr('disabled', 'disabled');
                        }

                        DiscountCodeController.ChangePhuongThucEdit();
                        $("#ModalEditDiscount").modal('show');
                    }
                }
            });


            $("#FromEditDiscount").off('submit').on('submit', function (e) {
                e.preventDefault();
                var percen2 = $('input[name = "flexRadioDefaultedit"]:checked').val();
                if (percen2 == 0) {
                    if ($("#Percentedit").val() == "") {
                        Toast.fire({
                            icon: 'error',
                            title: 'Chưa nhập phần trăm',
                        })
                    } else {
                        var discount = new Object();
                        discount.DiscountCodeID = id;
                        discount.Name = $("#NameDiscountedit").val();
                        discount.StartDate = $("#StartDateDiscountedit").val();
                        discount.EndDate = $("#EndDateDiscountedit").val();
                        discount.PercentCart = $("#Percentedit").val();
                        discount.TotalCart = $("#Totaledit").val().split('.').join("");
                        discount.DistcountCount = $("#CountDiscountedit").val();


                        $.ajax({
                            type: "post",
                            url: '/admin/DiscountCode/UpdateDiscountCode',
                            data: discount,
                            dataType: 'json',
                            success: function (data) {
                                if (data.code == 200) {
                                    if (data.data.Check == true) {
                                        $("#ModalEditDiscount").modal('hide');
                                        Toast.fire({
                                            icon: 'success',
                                            title: 'Thành công',
                                        })
                                        DiscountCodeController.lstdiscount();
                                        $('#paginationdiscount').twbsPagination('destroy');

                                    } else {
                                        Toast.fire({
                                            icon: 'error',
                                            title: data.data.Result,
                                        })
                                    }
                                }
                            }
                        });
                    }
                } else {
                    if ($("#Totaledit").val() == "") {
                        Toast.fire({
                            icon: 'error',
                            title: 'Chưa nhập tiền giảm',
                        })
                    } else {
                        var discount = new Object();
                        discount.DiscountCodeID = id;
                        discount.Name = $("#NameDiscountedit").val();
                        discount.StartDate = $("#StartDateDiscountedit").val();
                        discount.EndDate = $("#EndDateDiscountedit").val();
                        discount.PercentCart = $("#Percentedit").val();
                        discount.TotalCart = $("#Totaledit").val().split('.').join("");
                        discount.DistcountCount = $("#CountDiscountedit").val();


                        $.ajax({
                            type: "post",
                            url: '/admin/DiscountCode/UpdateDiscountCode',
                            data: discount,
                            dataType: 'json',
                            success: function (data) {
                                if (data.code == 200) {
                                    if (data.data.Check == true) {
                                        $("#ModalEditDiscount").modal('hide');
                                        Toast.fire({
                                            icon: 'success',
                                            title: 'Thành công',
                                        })
                                        DiscountCodeController.lstdiscount();
                                        $('#paginationdiscount').twbsPagination('destroy');

                                    } else {
                                        Toast.fire({
                                            icon: 'error',
                                            title: data.data.Result,
                                        })
                                    }
                                }
                            }
                        });
                    }
                }



            })


            function parseJsonDate(jsonDate) {

                var dt = new Date(parseInt(jsonDate.substr(6)));

                return `${dt.getFullYear().toString().padStart(4, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}`+'T'+`${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0')}`
            }
        })

    },
    ChangePhuongThucEdit: function () {
        $('input[type=radio][name=flexRadioDefaultedit]').change(function () {
            if (this.value == 0) {
                $('#Totaledit').attr('disabled', 'disabled');

                $('#Totaledit').val('');
                $('#Percentedit').removeAttr('disabled');
            }
            else if (this.value == 1) {

                $('#Totaledit').removeAttr('disabled');
                $('#Percentedit').val('');
                $('#Percentedit').attr('disabled', 'disabled');
            }
        });
    },
    deletediscount: function () {

        $(".actiondelete").off('click').on('click', function () {

            $("#ModalDelete").modal('show');

            var id = $(this).data('id');
            var name = $(this).data('name');

            $("#idnews").append('Bạn có chắc chắn xóa: '+(id))
            $("#namenews").append('Tên mã khuyến mãi: ' + name)
            $("#btndelete").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/DiscountCode/DeleteDiscountCode',
                    type: 'get',
                    data: {
                        id: id
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code = 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Xóa thành công',
                                })
                                $('#ModalDelete').modal('hide')
                                DiscountCodeController.lstdiscount();

                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Xóa thất bại',
                                })
                            }
                        }
                    }
                });
            })


        })
    }

}
DiscountCodeController.init();