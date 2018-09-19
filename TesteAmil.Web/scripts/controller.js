var controller = {

    TEMPLATE_PARTIDA: '<br><div class="row">' + 
        '<b>Partida</b>: #PARTIDA_ID#' +
            '<div class="col-sm-6 pull-right">' +
                'Data Início: #PARTIDA_DATA_INICIO#' +
                ' / Data Fim: #PARTIDA_DATA_FIM#' +
            '</div>' +
        '</div>' +
        '<div class="row">' +
                'Jogadores:' +
            '<table class="table table-responsive table-striped">' +
                '<thead>' +
                    '<tr>' +
                        '<th>Nome</th>' +
                        '<th>Matou</th>' +
                        '<th>Morreu</th>' +
                        '<th>Arma que mais Matou</th>' +
                        '<th>Awards</th>' +
                    '</tr>' +
                '</thead>' +
                '<tbody>' +
                    '#TABLEBODY#' +
                '</tbody>' +
            '</table>' +
        '</div>',
    Enviar: function () {
        
        if ($("#arquivoSelecionado").get(0).files.length == 0) {
            alert("Favor informar um arquivo de log.");
            return false;
        }

        var nomeArquivo = $("#arquivoSelecionado").val();
        var extensao = nomeArquivo.replace(/^.*\./, '');
        extensao = extensao.toLowerCase();
        var extensoesvalidas = ['log','txt']; 
        
        if ($.inArray(extensao, extensoesvalidas) == -1){
            alert("O formato do arquivo deverá estar na extensão '.log' ou '.txt'.");
            return false;
        }

        var dados = new FormData();
        dados.append("arquivo", $("#arquivoSelecionado").get(0).files[0]);

        $.ajax({
            type: "POST",
            url: "/Home/Processar",
            data: dados,
            processData: false,
            contentType: false,
            beforeSend: function () {
                $("#formArquivo").hide();
            },
            success: function (data) {

                if (data.sucesso == true)
                {
                    controller.Construir(data.data);
                }
                else {
                    $("#formArquivo").show();
                    alert("Ocorreu um erro ao processar arquivo, mensagem: " + data.mensagem);
                }
            },
            error: function (err) {
                $("#formArquivo").show();
                alert("Ocorreu um erro ao processar arquivo, por favor verificar se arquivo selecionado");
            }
        });

        
    },
    Construir(dados) {

        var tabelaJodadores = "";
        $("#ranking-items").empty();

        $.each(dados.Partidas, function (index,partida) {
            
            tabelaJodadores = "";

            $.each(partida.Jogadores, function (idx2, jogador) {

                var award_nao_morreu_partida = "";
                if (jogador.NumeroMortes == 0) {
                    award_nao_morreu_partida = "<span class='badge' style='background-color:#f5d76e;'>Não morreu na partida</span>"
                }

                tabelaJodadores +=  "<tr>" + 
                "<td>" + jogador.Nome + "</td>" + 
                "<td>" + jogador.NumeroAssassinatos + "</td>" + 
                "<td>" + jogador.NumeroMortes + "</td>" +
                "<td>" + jogador.ArmaPreferida + "</td>" +
                "<td>" + award_nao_morreu_partida  + "</td>" + 
                "</tr>"
            });

            var template = controller.TEMPLATE_PARTIDA.replace("#PARTIDA_ID#", partida.Id)
                                                      .replace("#PARTIDA_DATA_INICIO#", partida.DataInicio)
                                                      .replace("#PARTIDA_DATA_FIM#", partida.DataFim)
                                                      .replace("#TABLEBODY#", tabelaJodadores);

            $("#ranking-items").append(template);
        });

       


        $("#ranking").show();
    },
    Voltar: function () {
        $("#ranking").hide();
        $("#ranking-items").empty();
        $("#formArquivo").show();
    }



};
