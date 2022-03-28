
var pgcategory = {
    pageSize: 10,
    pageIndex: 1
}
var pgcategory2 = {
    pageSize: 2,
    pageIndex: 1
}

//Main
var CategoryController = {
    init: function () {
        //Khởi tạo theme
        CategoryController.initTheme();

        //Danh sách loại sản phẩm
        CategoryController.listCategoryBase();


        //Thêm Loại sản phẩm
        CategoryController.addCategory();

        //Search 
        CategoryController.searchCategoryBase();

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    chooseImageProduct: function () {
        $('#ImageCategoryChoose').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageCategory').val(url);
                $("#ImageShowCategory").removeAttr('src');
                $("#ImageShowCategory").attr('src', url);
            };
            finder.popup();
        });
    },
    chooseImageProductUpdate: function () {
        $('#ImageCategoryChooseUpdate').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#ImageCategoryUpdate').val(url);
                $("#ImageShowCategoryUpdate").removeAttr('src');
                $("#ImageShowCategoryUpdate").attr('src', url);
            };
            finder.popup();
        });
    },
    addCategory: function () {
        CategoryController.chooseImageProduct();
        $("#CreateCategoryBase").off('click').on('click', function () {
            $("#ModalCreateCategory").modal('show');

            $("#FromCreateCategory").off('submit').on('submit', function (e) {
                e.preventDefault();


                var category = new Object();
                category.Name = $("#NameCategory").val();
                category.Display = $("#DisplayCategory").val();
                category.Image = $("#ImageCategory").val();

                $.ajax({
                    type: "POST",
                    url: '/admin/productcategory/CreateCategoryBase',
                    data: category,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công!.',
                                })
                                $('#paginationcategory').twbsPagination('destroy');
                                $("#NameCategory").val('');
                                $("#DisplayCategory").val('');
                                $("#ImageCategory").val('');
                                CategoryController.listCategoryBase()
                                $('#ModalCreateCategory').modal('hide');
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
    addCategory2: function (id) {
        $("#FromCreateCategory2").off('submit').on('submit', function (e) {
            e.preventDefault();

            var category = new Object();
            category.Name = $("#CreateNameCategory2").val();
            category.Display = $("#CreateDisplayCategory2").val();
            category.ParentID = id;

            $.ajax({
                type: "POST",
                url: '/admin/productcategory/CreateCategory',
                data: category,
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data == true) {
                            Toast.fire({
                                icon: 'success',
                                title: 'Thêm thành công!.',
                            })
                            $('#paginationcategory2').twbsPagination('destroy');
                            $("#CreateNameCategory2").val('');
                            $("#CreateDisplayCategory2").val('');
                            CategoryController.listCategory2(id);

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
    },
    actionDisplay: function () {
        var flag = false;
        $('.actiondisplay').off('click').on('click', function () {
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
        })
    },
    listCategoryBase: function (changePageSize) {

        var search = new Object();
        search.SearchName = $("#search-querycategory").val();
        search.TuNgay = $("#TuNgayCategoryProduct").val();
        search.DenNgay = $("#DenNgayCategoryProduct").val();

        $.ajax({
            url: '/admin/productcategory/ListProductCategoryBase',
            type: 'post',
            data: {
                page: pgcategory.pageIndex,
                pageSize: pgcategory.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templatecategory').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ProductCategoryID: value.ProductCategoryID,
                            ProductCategoryIDDisplay: 'LSP' + ChangeIDProduct(value.ProductCategoryID),
                            Name: value.Name,
                            Display: value.Display,
                            CreateDate: parseJsonDate(value.CreateDate),
                            CountCategory: value.SLProduct != null ? value.SLProduct : 0,
                            ShowOnHome: value.ShowOnHome == true ? "fas fa-pause" : "fas fa-play",
                        });
                    });
                    $('#tbcategory').html(html);
                    $('#TongSoLuong').empty();
                    $('#TongSoLuong').append(data.countproduct);

                    CategoryController.paging(data.total, function () {
                        CategoryController.listCategoryBase();
                    }, changePageSize)

                    CategoryController.updateCategory();
                    CategoryController.deleteCategoryBase();
                    CategoryController.showcategory2();
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
    },
    searchCategoryBase: function () {
        $("#btnTiemKiemCategory").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationcategory').twbsPagination('destroy');
            CategoryController.listCategoryBase();
        })
    },
    showcategory2: function () {


        $('.modalshowcategory').off('click').on('click', function () {

            var id = $(this).data('id');

            $("#CategoryDisplayID").empty();

            $("#CategoryDisplayID").append('DANH SÁCH LOẠI SẢN PHẨM CỦA MÃ LOẠI SP: ' + 'LSP' + ChangeIDProduct(id))

            $("#ModalCategory2").modal('show');

            CategoryController.listCategory2(id);

            CategoryController.addCategory2(id);

            CategoryController.searchCategory2(id);

            $('#paginationcategory2').twbsPagination('destroy');


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
    listCategory2: function (id, changePageSize) {

        var search = new Object();
        search.SearchName = $("#search-querycategory2").val();
        search.TuNgay = $("#TuNgayCategoryProduct2").val();
        search.DenNgay = $("#DenNgayCategoryProduct2").val();


        $.ajax({
            url: '/admin/productcategory/ListProductCategoryCap2',
            type: 'post',
            data: {
                id: id,
                page: pgcategory2.pageIndex,
                pageSize: pgcategory2.pageSize,
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    console.log(data.data)
                    var html = "";
                    var template = $('#data-templatecategory2').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            ProductCategoryID: value.ProductCategoryID,
                            ProductCategoryIDDisplay: 'LSP' + ChangeIDProduct(value.ProductCategoryID),
                            Name: value.Name,
                            Display: value.Display,
                            CreateDate: parseJsonDate(value.CreateDate),
                            CountCategory: value.SLProduct != null ? value.SLProduct : 0,
                        });
                    });
                    $('#tbcategory2').html(html);
                    $('#TongSoLuong2').empty();
                    $('#TongSoLuong2').append(data.countproduct);

                    CategoryController.paging2(data.total, function () {
                        CategoryController.listCategory2(id);
                    }, changePageSize)

                    CategoryController.updateCategory2();
                    CategoryController.deleteCategory2(id);


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
    },
    searchCategory2: function (id) {
        $("#btnTiemKiemCategory2").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationcategory2').twbsPagination('destroy');
            CategoryController.listCategory2(id);
        })
    },
    updateCategory: function () {
        CategoryController.chooseImageProductUpdate();
        $('.modalshowEditcCategory').off('click').on('click', function () {
            $("#ModalUpdateCategory").modal('show');

            var id = $(this).data('id');

            $("#CategoryIDDisplayBase").val('LSP' + ChangeIDProduct(id));

            $.ajax({
                type: "GET",
                url: '/admin/productcategory/GetCategoryByID',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        $("#NameCategoryUpdate").val(data.data.Name);
                        $("#DisplayCategoryUpdate").val(data.data.Display);
                        $("#ImageCategoryUpdate").val(data.data.Image);
                        $("#ImageShowCategoryUpdate").removeAttr('src');
                        $("#ImageShowCategoryUpdate").attr('src', data.data.Image);
                    }
                }
            });

            $("#FromUpdateCategory").off('submit').on('submit', function (e) {
                e.preventDefault();

                var category = new Object();
                category.ProductCategoryID = id;
                category.Name = $("#NameCategoryUpdate").val();
                category.Display = $("#DisplayCategoryUpdate").val();
                category.Image = $("#ImageCategoryUpdate").val();
                $.ajax({
                    type: "POST",
                    url: '/admin/productcategory/UpdateCategoryBase',
                    data: category,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'cập nhật thành công',
                                })
                                $("#ModalUpdateCategory").modal('hide');
                                CategoryController.listCategoryBase()
                            } else {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'cập nhật thất bại',
                                })
                            }
                        }
                    }
                });

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
    updateCategory2: function () {
        $('.modalshowEditcCategory2').off('click').on('click', function () {
            $('#BtnUpdateCategory2').removeAttr('disabled');
            $('#BtnCreateCategory2').attr('disabled', 'disabled');

            var id = $(this).data('id');

            $.ajax({
                type: "GET",
                url: '/admin/productcategory/GetCategoryByID',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        $("#CategoryIDDisplay").val('LSP' + ChangeIDProduct(data.data.ProductCategoryID));
                        $("#UpdateNameCategory2").val(data.data.Name);
                        $("#UpdateDisplayCategory2").val(data.data.Display);
                    }
                }
            });

            $("#BtnUpdateCategory2").off('click').on('click', function (e) {
                e.preventDefault();

                var category = new Object();
                category.ProductCategoryID = id;
                category.Name = $("#UpdateNameCategory2").val();
                category.Display = $("#UpdateDisplayCategory2").val();
                $.ajax({
                    type: "POST",
                    url: '/admin/productcategory/UpdateCategory',
                    data: category,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data != null) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'cập nhật thành công',
                                })
                                $("#ModalUpdateCategory").modal('hide');
                                CategoryController.listCategory2(data.data);
                                $('#BtnUpdateCategory2').attr('disabled', 'disabled');
                                $('#BtnCreateCategory2').removeAttr('disabled');
                            } else {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'cập nhật thất bại',
                                })
                            }
                        }
                    }
                });

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
    deleteCategoryBase: function () {
        $(".actiondeleteCateogry1").off('click').on('click', function () {
            var id = $(this).data('id');

            var name = $(this).data('name');


            $("#NameProductCategoryDelete").empty();
            $("#NameProductCategoryDelete").append('Bạn có chắc chắn xóa: ' + name);

            $("#ResultProductCategoryDelete").attr("hidden", true);

            $("#ModalDeleteProductCategory").modal('show');

            $("#btndeleteProductCategory").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/productcategory/DeleteCategoryBase',
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
                                $('#ModalDeleteProductCategory').modal('hide')
                                CategoryController.listCategoryBase();

                            } else {
                                $("#ResultProductCategoryDelete").empty();
                                $("#ResultProductCategoryDelete").removeAttr("hidden");
                                $("#ResultProductCategoryDelete").append(data.data.Result);
                            }
                        }
                    }
                });
            })


        })

    },
    deleteCategory2: function (idcategory) {
        $(".actiondeleteCategory2").off('click').on('click', function (e) {
            var id = $(this).data('id');

            e.preventDefault()
            $.ajax({
                url: '/admin/productcategory/DeleteCategoryCap2',
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
                            CategoryController.listCategory2(idcategory);

                        } else {
                            Toast.fire({
                                icon: 'error',
                                title: data.data.Result,
                            })
                        }
                    }
                }
            });

        })
    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgcategory.pageSize)
        if ($('#paginationcategory a').length === 0 || changePageSize === true) {
            $('#paginationcategory').empty();
            $('#paginationcategory').removeData('twbs-pagination');
            $('#paginationcategory').unbind("page");
        }
        $('#paginationcategory').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgcategory.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },

    paging2: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgcategory2.pageSize)
        if ($('#paginationcategory2 a').length === 0 || changePageSize === true) {
            $('#paginationcategory2').empty();
            $('#paginationcategory2').removeData('twbs-pagination');
            $('#paginationcategory2').unbind("page");
        }
        $('#paginationcategory2').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgcategory2.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },


}
CategoryController.init();