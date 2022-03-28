var pg = {
    pageSize: 2,
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
var NewsController = {
    init: function () {
        //Khởi tạo theme
        NewsController.initTheme();
        //Tạo tin tức
        NewsController.CreateNew();

        //Danh sách tin tức
        NewsController.lstNews();

        //Tìm kiếm
        NewsController.searchNews();


        //Danh sách loại tin tức
        NewsController.lstcategorynews();

        //Tìm kiếm loại tin
        NewsController.searchcategory();

        //Thêm loại tin tức
        NewsController.CreateNewsCategory();
    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },

    chooseImageNews: function () {
        $('#imagechoosenews').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#imagenews').val(url);
                $("#ImageShownew").removeAttr('src');
                $("#ImageShownew").attr('src', url);
            };
            finder.popup();
        });
    },
    CreateNew: function () {

        NewsController.chooseImageNews();
        NewsController.lstcateogrynew();
        $("#CreateNews").off('click').on('click', function () {

            CKEDITOR.replace('detailnew', {
                customConfig: '/Assets/Admin/plugins/ckeditor/config.js'
            });

            $("#ModalCreateNews").modal('show');
        
            $("#FromCreateNews").off('submit').on('submit', function (e) {
                e.preventDefault();
                var txt = CKEDITOR.instances['detailnew'].getData();

                var news = new Object();
                news.Name = $("#NewName").val();
                news.Detail = txt;
                news.Image = $("#imagenews").val();
                news.CategoryNewID = $("#categorynews").val();


                $.ajax({
                    type: "post",
                    url: '/admin/news/CreateNew',
                    data: news,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                $("#ModalCreateNews").modal('hide');

                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                NewsController.lstNews();
                                $('#paginationCategoryNews').twbsPagination('destroy');

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
        })
    },
    lstNews: function (changePageSize) {
        
        var search = new Object();
        search.SearchName = $("#search-news").val();
        search.TuNgay = $("#TuNgay").val();
        search.DenNgay = $("#DenNgay").val();

        $.ajax({
            url: '/admin/news/GetAllAdmin',
            type: 'post',
            data: {                
                page: pg.pageIndex,
                pageSize: pg.pageSize,
                searchnews: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templateNews').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            NewsIDDisplay: 'TT' + ChangeIDProduct(value.NewsID),
                            NewsID: value.NewsID,
                            CreateDate: parseJsonDate(value.CreateDate),
                            Name: value.Name,
                            UserName: value.NameUser,
                            CategoryName: value.CategoryName
                        });
                    });
                    $('#tbNews').html(html);
                    if (data.total != 0) {
                        NewsController.paging(data.total, function () {
                            NewsController.lstNews();
                        }, changePageSize)
                    }
                }

                NewsController.editcategorynew();
                NewsController.deletenews();
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
    },
    searchNews: function () {

        $("#btnTiemKiem").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationnews').twbsPagination('destroy');
            NewsController.lstNews();

        })
    },
    editcategorynew: function () {
        NewsController.lstcateogrynewedit();
        $(".modalshowEdit").off('click').on('click', function () {
            CKEDITOR.replace('detailnewedit', {
                customConfig: '/Assets/Admin/plugins/ckeditor/config.js'
            });
            var id = $(this).data('id');           
            $.ajax({
                url: '/admin/news/GetDetail',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        $("#NewsIDDisplay").val('TT'+ChangeIDProduct(data.data.NewsID));
                        $("#NewNameEdit").val(data.data.Name);
                        $("#ImageShownewedit").removeAttr('src');
                        $("#ImageShownewedit").attr('src', data.data.Image);
                        $("#imagenewsedit").val(data.data.Image);
                        $(`#categorynewsedit option[value='${data.data.CategoryNewID}']`).prop('selected', true);
                        
                        if (data.data.Detail != null || data.data.Detail != '') {
                            CKEDITOR.instances['detailnewedit'].setData(data.data.Detail);
                        }
                        $("#ModalEditNews").modal('show');
                    }
                }
            });

            $("#FromEditNews").off('submit').on('submit', function (e) {

                e.preventDefault();
                var txt = CKEDITOR.instances['detailnewedit'].getData();

                var news = new Object();
                news.NewsID = id;
                news.Name = $("#NewNameEdit").val();
                news.Detail = txt;
                news.Image = $("#imagenewsedit").val();
                news.CategoryNewID = $("#categorynewsedit").val();

                $.ajax({
                    type: "post",
                    url: '/admin/news/UpdateNews',
                    data: news,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Cập nhật thành công',
                                })
                                NewsController.lstNews();
                                $("#ModalEditNews").modal('hide');
                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Cập nhật Thất bại',
                                })
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
    deletenews: function () {
        $(".actiondelete").off('click').on('click', function () {
            $("#ModalDelete").modal('show');

            var id = $(this).data('id');
            var name = $(this).data('name');

            $("#idnews").append('Bạn có chắc chắn xóa: ' + 'TT'+ChangeIDProduct(id))
            $("#namenews").append('Tiêu đề: ' + name)
            $("#btndelete").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/news/DeleteNew',
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
                                NewsController.lstNews();

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
    lstcateogrynew: function () {

        $.ajax({
            url: '/admin/news/GetAllCategoryNew',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#categorynews');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.CategoryNewID).text(val.Name).appendTo($select);
                    });
                }
            }
        });
    },
    lstcateogrynewedit: function () {

        $.ajax({
            url: '/admin/news/GetAllCategoryNew',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#categorynewsedit');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.CategoryNewID).text(val.Name).appendTo($select);
                    });
                }
            }
        });
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
    lstcategorynews: function (changePageSize) {
        var search = new Object();
        search.SearchName = $("#search-newscategory").val();
        search.TuNgay = $("#TuNgayCategory").val();
        search.DenNgay = $("#DenNgayCategory").val();

        $.ajax({
            url: '/admin/news/GetAllCategoryNewPaging',
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
                    var template = $('#data-templateCategoryNews').html();
                    var dem = 0;
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            CategoryNewIDDisplay: 'LTT' + ChangeIDProduct(value.CategoryNewID),
                            CategoryNewID: value.CategoryNewID,
                            CreateDate: parseJsonDate(value.CreateDate),
                            Name: value.Name,
                            Display: value.Display,
                            CategoryStatus: value.CategoryStatus,
                            CountNews: value.CountNews
                        });
                        dem += value.CountNews;
                    });
                    $('#counttotalnews').empty();
                    $("#counttotalnews").append(dem);

                    $('#tbCategoryNews').html(html);
                    if (data.total != 0) {
                        NewsController.pagingcategory(data.total, function () {
                            NewsController.lstcategorynews();
                        }, changePageSize)
                    }
                    NewsController.EditCategory();
                    NewsController.DeleteCategory();
                }
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
    },
    searchcategory: function () {
        $("#btnTiemKiemCategory").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationCategoryNews').twbsPagination('destroy');
            NewsController.lstcategorynews();

        })
    },
    CreateNewsCategory: function () {
        $("#CreateNewsCategory").off('click').on('click', function () {
            $("#ModalCreateNewsCategory").modal('show');

            $("#FromCreateNewsCategory").off('submit').on('submit', function (e) {
                e.preventDefault();

                var category = new Object();
                category.Name = $("#CategoryName").val();
                category.Display = $("#Displaycategory").val();

                $.ajax({
                    type: "post",
                    url: '/admin/news/CreateCategorynew',
                    data: category,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                $("#ModalCreateNewsCategory").modal('hide');
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                $('#paginationCategoryNews').twbsPagination('destroy');
                                $('#CategoryName').val('');
                                $('#Displaycategory').val('');
                                NewsController.lstcategorynews();
                                NewsController.lstcateogrynew();

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
        })

    },
    EditCategory: function () {
        $(".modalshowEditCategory").off('click').on('click', function () {
            var id = $(this).data('id');
            $.ajax({
                url: '/admin/news/DetailCategory',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        $("#CategoryIDDisplay").val('LTT' + ChangeIDProduct(data.data.CategoryNewID));
                        $("#CategoryNameEdit").val(data.data.Name);
                        $("#DisplaycategoryEdit").val(data.data.Display);
                        $("#ModalEditNewsCategory").modal('show');
                    }
                }
            });
            $("#FromEditNewsCategory").off('submit').on('submit', function (e) {

                e.preventDefault();

                var category = new Object();
                category.CategoryNewID = id;
                category.Name = $("#CategoryNameEdit").val();
                category.Display = $("#DisplaycategoryEdit").val();

                $.ajax({
                    type: "post",
                    url: '/admin/news/EditCategorynew',
                    data: category,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                $("#ModalEditNewsCategory").modal('hide');
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                NewsController.lstcategorynews();

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
    DeleteCategory: function () {
        $(".actiondeleteCategory").off('click').on('click', function () {
            $("#ModalDeleteCategory").modal('show');
            var id = $(this).data('id');
            var name = $(this).data('name');

            $("#idnewscategory").append('Bạn có chắc chắn xóa: ' + 'LTT' + ChangeIDProduct(id))
            $("#namenewscategory").append('Tên loại: ' + name)

            $("#btndeleteCateogry").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/news/DeleteCategory',
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
                                $('#paginationCategoryNews').twbsPagination('destroy');
                                $('#ModalDeleteCategory').modal('hide')
                                NewsController.lstcategorynews();
                                NewsController.lstcateogrynew();
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
    pagingcategory: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgcategory.pageSize)
        if ($('#paginationCategoryNews a').length === 0 || changePageSize === true) {
            $('#paginationCategoryNews').empty();
            $('#paginationCategoryNews').removeData('twbs-pagination');
            $('#paginationCategoryNews').unbind("page");
        }
        $('#paginationCategoryNews').twbsPagination({
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
}
NewsController.init();