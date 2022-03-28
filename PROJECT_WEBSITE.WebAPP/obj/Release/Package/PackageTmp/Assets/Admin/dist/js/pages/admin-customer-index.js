var pg = {
    pageSize: 10,
    pageIndex: 1
}

var pgcategory = {
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
var CustomerController = {
    init: function () {

        CustomerController.lstNews();

        CustomerController.searchNews();

    },
    actionDisplayProduct: function () {
        var flag = false;
        $('.actiondisplayProduct').off('click').on('click', function () {
            var id = $(this).data('id');
            console.log('da click' + id);
            if (flag == false) {
                $('#actiondisplay').toggle('1000');
                icon = $(this).find("i");
                icon.toggleClass("fas fa-lock-open fas fa-lock")
                flag = true;
            }
            else {
                $('#actiondisplay').hide('1000');
                $('#actiondisplay').toggle('1000');
                icon = $(this).find("i");
                icon.toggleClass("fas fa-lock fas fa-lock-open ")
                flag = false;
            }

            $.ajax({
                type: "get",
                url: '/admin/customer/ChangeStatusCustomer',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data.Check == true) {
                            Toast.fire({
                                icon: 'success',
                                title: 'Mở khóa tài khoản',
                            })
                            CustomerController.lstNews();
                        } else {
                            Toast.fire({
                                icon: 'success',
                                title: 'Khóa tài khoản',
                            })
                            CustomerController.lstNews();
                        }
                    }
                }
            });


        })
    },
    lstNews: function (changePageSize) {

        var search = new Object();
        search.SearchName = $("#search-news").val();
        search.TuNgay = $("#TuNgay").val();
        search.DenNgay = $("#DenNgay").val();

        $.ajax({
            url: '/admin/customer/GetAllPaging',
            type: 'post',
            data: {
                page: pg.pageIndex,
                pageSize: pg.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    console.log(data.data)
                    var html = "";
                    var template = $('#data-templateNews').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            CustomerIDDisplay: 'KH' + ChangeIDProduct(value.CustomerID),
                            CustomerID: value.CustomerID,
                            Name: value.Name,
                            CreateDate: parseJsonDate(value.CreateDate),
                            Phone: value.Phone,
                            Address: value.Address,
                            Email: value.Email,
                            Birth: parseJsonDate(value.Birth),
                            Gender: value.Gender,
                            Status: Status(value.CustomerStatus),
                            DisplayProduct: value.CustomerStatus == 1 ? "fas fa-lock" : "fas fa-lock-open",
                        });
                    });
                    $('#tbNews').html(html);
                    if (data.total != 0) {
                        CustomerController.paging(data.total, function () {
                            CustomerController.lstNews();
                        }, changePageSize)
                    }
                }
                CustomerController.actionDisplayProduct();
                CustomerController.deletenews();
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
        function Status(value) {
            if (value == 0) {
                return "<span class=\"badge bg-danger\">Khóa</span>";
            }
            if (value == 1) {
                return "<span class=\"badge bg-primary\">Hoạt động</span>";
            }
        }
    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#paginationnews a').length === 0 || changePageSize === true) {
            $('#paginationnews').empty();
            $('#paginationnews').removeData('twbs-pagination');
            $('#paginationnews').unbind("page");
        }
        $('#paginationnews').twbsPagination({
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
    deletenews: function () {
        $(".actiondelete").off('click').on('click', function () {

            var id = $(this).data('id');

            var name = $(this).data('name');

            $("#NameProductDelete").empty();
            $("#NameProductDelete").append('Bạn có chắc chắn xóa: ' + name);

            $("#ResultProductDelete").attr("hidden", true);

            $("#ModalDeleteProduct").modal('show');

            $("#btndeleteProduct").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/customer/DeleteCustomer',
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
    searchNews: function () {

        $("#btnTiemKiem").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationnews').twbsPagination('destroy');
            CustomerController.lstNews();

        })
    },

}
CustomerController.init();