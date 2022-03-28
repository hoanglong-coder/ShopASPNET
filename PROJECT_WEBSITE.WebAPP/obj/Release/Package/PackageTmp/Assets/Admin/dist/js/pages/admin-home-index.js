//Main
var HomeController = {
    init: function () {
        //Khởi tạo theme
        HomeController.initTheme();

        HomeController.lstngay();

        HomeController.lableday();

        HomeController.changeSelect();

        HomeController.Search();

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    lstngay: function () {
        $.ajax({
            url: '/admin/Home/GetListChonThoiGian',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#chonthoigian');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.Value).text(val.Name).appendTo($select);
                    });
                }
            }
        });
    }
    ,
    lableday: function () {

        var search = new Object();
        search.SearchName = "12";
        search.TuNgay = $("#TuNgay").val();
        search.DenNgay = $("#DenNgay").val();

        $.ajax({
            url: '/admin/Home/GetLabelDay',
            type: 'post',
            data: {
                search: search
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    $("#ngaychon").empty();
                    $("#ngaychon").append(data.data);

                    if (search.TuNgay != "" && search.DenNgay != "") {

                        $("#ngaychon").empty();


                        $("#ngaychon").append('(' + parseJsonDate(data.search.TuNgay) + " - " + parseJsonDate(data.search.DenNgay)+')');

                    }


                    $("#DoanhSo").empty();
                    $("#DoanhSo").append(new Intl.NumberFormat().format(data.DoanhSo) + ' VNĐ');

                    $("#DonHang").empty();
                    $("#DonHang").append(data.DonHang);

                    $("#Soluong").empty();
                    $("#Soluong").append(data.Soluong);

                    $("#laigop").empty();
                    $("#laigop").append(new Intl.NumberFormat().format(data.LaiGop) + ' VNĐ');

                    $("#TongKho").empty();
                    $("#TongKho").append(data.TongKho);

                    $("#TonKho").empty();
                    $("#TonKho").append(data.TonKho);

                    $("#HangHet").empty();
                    $("#HangHet").append(data.Dahet);


                    $("#chartPie").remove();

                    $("#chartPie_box").after("<canvas id='chartPie'></canvas>");


                    //chartpie
                    const labelspie = data.lstpie;

                    const data3 = {
                        labels: labelspie,
                        datasets: [{
                            label: 'Kho hàng',
                            data: data.lstvalue,
                            backgroundColor: [
                                '#007bff',
                                '#28a745',
                                '#dc3545'
                            ],
                        }]
                    };

                    const configpie = {
                        type: 'pie',
                        data: data3,
                        options: {}
                    };

                    const myChartPie = new Chart(
                        document.getElementById('chartPie'),
                        configpie
                    );

                    $("#chartDoanhSo").remove();

                    $("#chart_box").after("<canvas id='chartDoanhSo'></canvas>");

                    const labels = data.LabelDoanhSo;

                    const data2 = {
                        labels: labels,
                        datasets: [{
                            label: 'Doanh số bán',
                            backgroundColor: 'rgb(255, 99, 132)',
                            borderColor: 'rgb(255, 99, 132)',
                            data: data.ValueDoanhSo,
                        }]
                    };

                    const config = {
                        type: 'line',
                        data: data2,
                        options: {}
                    };

                    const myChart = new Chart(
                        document.getElementById('chartDoanhSo'),
                        config
                    );




                    $("#chartSoluong").remove();

                    $("#chart_boxSoluong").after("<canvas id='chartSoluong'></canvas>");

                    const datasoluong = {
                        labels: labels,
                        datasets: [{
                            label: 'Số lượng bán',
                            backgroundColor: 'rgb(255, 99, 132)',
                            borderColor: 'rgb(255, 99, 132)',
                            data: data.ValueSoluong,
                        }]
                    };

                    const config2 = {
                        type: 'line',
                        data: datasoluong,
                        options: {}
                    };

                    const myChart2 = new Chart(
                        document.getElementById('chartSoluong'),
                        config2
                    );
                }
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')}`
        }
    }
    ,
    changeSelect: function () {
        $("#chonthoigian").on('change', function (e) {
            var optionSelected = $("option:selected", this);
            var valueSelected = this.value;

            $.ajax({
                url: '/admin/Home/GetLabelDay',
                type: 'post',
                data: {
                    value: valueSelected
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        $("#ngaychon").empty();
                        $("#ngaychon").append(data.data);

                        $("#DoanhSo").empty();
                        $("#DoanhSo").append(new Intl.NumberFormat().format(data.DoanhSo) + ' VNĐ');

                        $("#DonHang").empty();
                        $("#DonHang").append(data.DonHang);

                        $("#Soluong").empty();
                        $("#Soluong").append(data.Soluong);

                        $("#laigop").empty();
                        $("#laigop").append(new Intl.NumberFormat().format(data.LaiGop) + ' VNĐ');

                        $("#TongKho").empty();
                        $("#TongKho").append(data.TongKho);

                        $("#TonKho").empty();
                        $("#TonKho").append(data.TonKho);

                        $("#HangHet").empty();
                        $("#HangHet").append(data.Dahet);

                        $("#chartDoanhSo").remove();

                        $("#chart_box").after("<canvas id='chartDoanhSo'></canvas>");

                        const labels = data.LabelDoanhSo;

                        const data2 = {
                            labels: labels,
                            datasets: [{
                                label: 'Doanh số bán',
                                backgroundColor: 'rgb(255, 99, 132)',
                                borderColor: 'rgb(255, 99, 132)',
                                data: data.ValueDoanhSo,
                            }]
                        };

                        const config = {
                            type: 'line',
                            data: data2,
                            options: {}
                        };

                        const myChart = new Chart(
                            document.getElementById('chartDoanhSo'),
                            config
                        );


                        $("#chartSoluong").remove();

                        $("#chart_boxSoluong").after("<canvas id='chartSoluong'></canvas>");

                        const datasoluong = {
                            labels: labels,
                            datasets: [{
                                label: 'Số lượng bán',
                                backgroundColor: 'rgb(255, 99, 132)',
                                borderColor: 'rgb(255, 99, 132)',
                                data: data.ValueSoluong,
                            }]
                        };

                        const config2 = {
                            type: 'line',
                            data: datasoluong,
                            options: {}
                        };

                        const myChart2 = new Chart(
                            document.getElementById('chartSoluong'),
                            config2
                        );

                    }
                }
            });

        });
    }
    ,
    Search: function () {

        $("#btnTiemKiem").off('click').on('click', function () {

            HomeController.lableday();

        })
    }
}
HomeController.init();