var pgUnit = {
    pageSize: 10,
    pageIndex: 1
}


//Main
var ProductUnitController = {
    init: function () {
        //Khởi tạo theme
        ProductUnitController.initTheme();

        //Danh sách đơn vị tính
        ProductUnitController.listProductUnitBase();

        //Thêm đơn vị tính
        ProductUnitController.addProductUnit();

        //Search
        ProductUnitController.searchUnit();
    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    addProductUnit: function () {
        $('#CreateUnit').off('click').on('click', function () {

            $("#ModalCreateUnit").modal('show');

            $("#FromCreateUnit").off('submit').on('submit', function (e) {
                e.preventDefault();

                var Unit = new Object();
                Unit.Name = $("#NameUnit").val();
                Unit.ValueUnit = $("#ValueUnit").val();
                $.ajax({
                    type: "POST",
                    url: '/admin/productunit/CreateUnit',
                    data: Unit,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công',
                                })
                                $("#ModalCreateUnit").modal('hide');
                                $('#paginationproductUnit').twbsPagination('destroy');
                                ProductUnitController.listProductUnitBase();
                            } else {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thất bại',
                                })
                            }
                        }
                    }
                });
            })
        });
    },
    deleteUnit: function () {
        $(".actiondeleteUnit").off('click').on('click', function () {
            var id = $(this).data('id');

            var name = $(this).data('name');

            var value = $(this).data('value');

            $("#NameUnitDelete").empty();
            $("#NameUnitDelete").append('Bạn có chắc chắn xóa: ' + name + ' - Giá trị: ' + value);

            $("#ResultUnitDelete").attr("hidden", true);

            $("#ModalDeleteUnit").modal('show');

            $("#btndeleteUnit").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/productunit/DeleteUnit',
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
                                $('#ModalDeleteUnit').modal('hide')
                                ProductUnitController.listProductUnitBase();
                                $("#paginationproductUnit").twbsPagination('destroy');

                            } else {
                                $("#ResultUnitDelete").empty();
                                $("#ResultUnitDelete").removeAttr("hidden");
                                $("#ResultUnitDelete").append(data.data.Result);
                            }
                        }
                    }
                });
            })

        })
    },
    listProductUnitBase: function (changePageSize) {
        var search = new Object();
        search.query = $("#search-queryUnit").val();
        search.LoaiDVT = $("#search-SelectUnit").val();
        search.GiaTri = $("#search-giatri").val();

        $.ajax({
            url: '/admin/productunit/ListProductUnitAll',
            type: 'post',
            data: {
                page: pgUnit.pageIndex,
                pageSize: pgUnit.pageSize,
                search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templateProductUnit').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ID: value.UnitID,
                            UnitID: 'DVT' +ChangeIDProduct(value.UnitID),
                            Name: value.Name,
                            ValueUnit: value.ValueUnit,
                        });
                    });
                    $('#tbProductUnit').html(html);
                    ProductUnitController.paging(data.total, function () {
                        ProductUnitController.listProductUnitBase();
                    }, changePageSize)

                    ProductUnitController.updateUnit();
                    ProductUnitController.deleteUnit();
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
    searchUnit: function () {
        $("#btnsearchUnit").off('click').on('click', function () {

            $("#paginationproductUnit").twbsPagination('destroy');
            ProductUnitController.listProductUnitBase();
        })
    },
    updateUnit: function () {
        $('.modalshowEditUnit').off('click').on('click', function () {
            $("#ModalUpdateUnit").modal('show');

            var iddisplay = $(this).data('iddisplay');
            var id = $(this).data('id');
            $("#UnitIDUpdate").val(iddisplay);

            $.ajax({
                type: "GET",
                url: '/admin/productunit/GetUnitByID',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        $("#NameUnitUpdate").val(data.data.Name);
                        $("#ValueUnitUpdate").val(data.data.ValueUnit);                        
                    }
                }
            });

            $("#FromUpdateUnit").off('submit').on('submit', function (e) {
                e.preventDefault();

                var Unit = new Object();
                Unit.UnitID = id;
                Unit.Name = $("#NameUnitUpdate").val();
                Unit.ValueUnit = $("#NameUnitUpdate").val();
                $.ajax({
                    type: "POST",
                    url: '/admin/productunit/UpdateUnit',
                    data: Unit,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công',
                                })
                                $("#ModalUpdateUnit").modal('hide');
                                ProductUnitController.listProductUnitBase();
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
        });
    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgUnit.pageSize)
        if ($('#paginationproductUnit a').length === 0 || changePageSize === true) {
            $('#paginationproductUnit').empty();
            $('#paginationproductUnit').removeData('twbs-pagination');
            $('#paginationproductUnit').unbind("page");
        }
        $('#paginationproductUnit').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgUnit.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },

}
ProductUnitController.init();