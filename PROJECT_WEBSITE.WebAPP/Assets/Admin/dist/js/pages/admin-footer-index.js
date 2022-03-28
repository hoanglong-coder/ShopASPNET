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
var FooterController = {
    init: function () {
        FooterController.CreateNew();

        FooterController.lstNews();

        FooterController.searchNews();


        FooterController.lstcategorynews();

        FooterController.CreateNewsCategory();

        FooterController.searchcategory();

    },
    CreateNew: function () {
        FooterController.lstcateogrynew();
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
                news.FooterCategoryID = $("#categorynews").val();


                $.ajax({
                    type: "post",
                    url: '/admin/footer/CreateFooter',
                    data: news,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                $("#ModalCreateNews").modal('hide');

                                $("#NewName").val('');

                                CKEDITOR.instances['detailnew'].setData('');

                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                $('#paginationnews').twbsPagination('destroy');
                                FooterController.lstNews();                         

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
    lstcateogrynew: function () {

        $.ajax({
            url: '/admin/footer/GetAllCategoryFooter',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#categorynews');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.FooterCategoryID).text(val.NameCategory).appendTo($select);
                    });
                }
            }
        });
    },
    lstNews: function (changePageSize) {

        var search = new Object();
        search.SearchName = $("#search-news").val();
        search.TuNgay = $("#TuNgay").val();
        search.DenNgay = $("#DenNgay").val();

        $.ajax({
            url: '/admin/footer/GetFooter',
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
                    var template = $('#data-templateNews').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            FooterID: value.FooterID,
                            CreateDate: parseJsonDate(value.CreateDate),
                            Name: value.Name,
                            FooterCategoryList: value.FooterCategoryName,
                        });
                    });
                    $('#tbNews').html(html);
                    if (data.total != 0) {
                        FooterController.paging(data.total, function () {
                            FooterController.lstNews();
                        }, changePageSize)
                    }
                }

                FooterController.editcategorynew();
                FooterController.deletenews();
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
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
    editcategorynew: function () {
        FooterController.lstcateogrynewedit();
        $(".modalshowEdit").off('click').on('click', function () {
            CKEDITOR.replace('detailnewedit', {
                customConfig: '/Assets/Admin/plugins/ckeditor/config.js'
            });
            var id = $(this).data('id');
            $.ajax({
                url: '/admin/footer/GetDetail',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        $("#NewsIDDisplay").val(data.data.FooterID);
                        $("#NewNameEdit").val(data.data.Name);
                        $(`#categorynewsedit option[value='${data.data.FooterCategoryID}']`).prop('selected', true);

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
                news.FooterID = id;
                news.Name = $("#NewNameEdit").val();
                news.Detail = txt;
                news.FooterCategoryID = $("#categorynewsedit").val();

                $.ajax({
                    type: "post",
                    url: '/admin/footer/UpdateFooter',
                    data: news,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Cập nhật thành công',
                                })
                                FooterController.lstNews();
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
    },
    lstcateogrynewedit: function () {
        $.ajax({
            url: '/admin/footer/GetAllCategoryFooter',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#categorynewsedit');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.FooterCategoryID).text(val.NameCategory).appendTo($select);
                    });
                }
            }
        });
    },
    deletenews: function () {
        $(".actiondelete").off('click').on('click', function () {
            $("#ModalDelete").modal('show');

            var id = $(this).data('id');
            var name = $(this).data('name');

            $("#idnews").append('Bạn có chắc chắn xóa: ' + id)
            $("#namenews").append('Tiêu đề: ' + name)
            $("#btndelete").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/footer/Delete',
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
                                $('#paginationnews').twbsPagination('destroy');
                                FooterController.lstNews();

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
    },
    searchNews: function () {

        $("#btnTiemKiem").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationnews').twbsPagination('destroy');
            FooterController.lstNews();

        })
    },

    lstcategorynews: function (changePageSize) {
        var search = new Object();
        search.SearchName = $("#search-newscategory").val();
        search.TuNgay = $("#TuNgayCategory").val();
        search.DenNgay = $("#DenNgayCategory").val();

        $.ajax({
            url: '/admin/footer/GetAllPaging',
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
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            FooterCategoryID: value.FooterCategoryID,
                            NameCategory: value.NameCategory,
                            CreateDate: parseJsonDate(value.CreateDate),
                            Display: value.Display
                        });
                    });

                    $('#tbCategoryNews').html(html);
                    if (data.total != 0) {
                        FooterController.pagingcategory(data.total, function () {
                            FooterController.lstcategorynews();
                        }, changePageSize)
                    }
                    FooterController.EditCategory();
                    FooterController.DeleteCategory();
                }
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    searchcategory: function () {
        $("#btnTiemKiemCategory").off('click').on('click', function (e) {
            e.preventDefault();
            $('#paginationCategoryNews').twbsPagination('destroy');
            FooterController.lstcategorynews();

        })
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
    CreateNewsCategory: function () {
        $("#CreateNewsCategory").off('click').on('click', function () {
            $("#ModalCreateNewsCategory").modal('show');

            $("#FromCreateNewsCategory").off('submit').on('submit', function (e) {
                e.preventDefault();

                var category = new Object();
                category.NameCategory = $("#CategoryName").val();
                category.Display = $("#Displaycategory").val();

                $.ajax({
                    type: "post",
                    url: '/admin/footer/CreateFooterCate',
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
                                FooterController.lstcategorynews();
                                FooterController.lstcateogrynew();

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
                url: '/admin/footer/GetDetailCate',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        $("#CategoryIDDisplay").val(data.data.FooterCategoryID);
                        $("#CategoryNameEdit").val(data.data.NameCategory);
                        $("#DisplaycategoryEdit").val(data.data.Display);
                        $("#ModalEditNewsCategory").modal('show');
                    }
                }
            });
            $("#FromEditNewsCategory").off('submit').on('submit', function (e) {

                e.preventDefault();

                var category = new Object();
                category.FooterCategoryID = id;
                category.NameCategory = $("#CategoryNameEdit").val();
                category.Display = $("#DisplaycategoryEdit").val();

                $.ajax({
                    type: "post",
                    url: '/admin/footer/UpdateFooterCate',
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
                                FooterController.lstcategorynews();
                                FooterController.lstcateogrynewedit();
                                FooterController.lstcateogrynew();

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
    DeleteCategory: function () {
        $(".actiondeleteCategory").off('click').on('click', function () {
            $("#ModalDeleteCategory").modal('show');
            var id = $(this).data('id');
            var name = $(this).data('name');

            $("#idnewscategory").append('Bạn có chắc chắn xóa: '+id)
            $("#namenewscategory").append('Tên loại: ' + name)

            $("#btndeleteCateogry").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/footer/DeleteCate',
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
                                FooterController.lstcategorynews();
                                FooterController.lstcateogrynew();
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
    },
}
FooterController.init();