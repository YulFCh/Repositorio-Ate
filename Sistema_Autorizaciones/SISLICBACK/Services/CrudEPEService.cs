
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders;
using SistemaLicencias.SHARED.DTOs;
using SistemaLicencias.SHARED.DTOs.EPE;
using System.Data;
using System.Reflection;

namespace SISLICBACK.Services {
    public class CrudEPEService {
        private static readonly HttpClient client = new HttpClient();
        private string _conn;

        public CrudEPEService(IConfiguration _configuration) {
            _conn = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
        }


        public async Task<CrearAnuncioResult> CrearAnuncioConDetalleAsync(DetalleAnuncioInput det, AnuncioInput an, CancellationToken ct = default) {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("Publicidad.usp_ins_anuncio_y_detalle", cn) {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            // ------- helpers locales -------
            static object DbNullIfNull(string? v) => string.IsNullOrWhiteSpace(v) ? DBNull.Value : v;

            static void AddVarchar(SqlCommand c, string name, int size, string? value) {
                var p = c.Parameters.Add(name, SqlDbType.VarChar, size);
                p.Value = DbNullIfNull(value);
            }

            static void AddInt(SqlCommand c, string name, int? value) {
                var p = c.Parameters.Add(name, SqlDbType.Int);
                p.Value = value.HasValue ? value.Value : DBNull.Value;
            }
          
            static void AddDateTime2(SqlCommand c, string name, DateTime? value) {
                var p = c.Parameters.Add(name, SqlDbType.DateTime2);
                p.Value = value.HasValue ? value.Value : DBNull.Value;
            }

            // --------- DetalleAnuncio ----------
            AddVarchar(cmd, "@direccion", 200, det.direccion);
            AddVarchar(cmd, "@direcnro", 20, det.direcnro);
            AddVarchar(cmd, "@direcint", 20, det.direcint);
            AddVarchar(cmd, "@direcmz", 20, det.direcmz);
            AddVarchar(cmd, "@direclt", 20, det.direclt);
            AddVarchar(cmd, "@direcdenomina", 20, det.direcdenomina);
            AddVarchar(cmd, "@altoa", 10, det.alto);
            AddVarchar(cmd, "@ancho", 10, det.ancho);
            AddVarchar(cmd, "@area", 10, det.area);
            AddVarchar(cmd, "@nrocaras", 10, det.nrocaras);
            AddVarchar(cmd, "@zonaUrbana", 100, det.zonaUrbana);
            AddVarchar(cmd, "@zonificacion", 50, det.zonificacion);
            AddVarchar(cmd, "@coordLngLat", 50, det.coordLngLat);
            AddVarchar(cmd, "@horario", 50, det.horario);
            AddInt(cmd, "@idLicencia", det.idLicencia);
            AddVarchar(cmd, "@nroLicencia", 10, det.nroLicencia);
            AddVarchar(cmd, "@anio", 10, det.anio);
            AddVarchar(cmd, "@distrito", 50, det.distrito);
            AddVarchar(cmd, "@razsocial", 200, det.razsocial);


            // --------- AnuncioPublicitario ----------
            AddVarchar(cmd, "@nroAutorizacion", 11, an.nroAutorizacion);
            AddVarchar(cmd, "@fechaAutorizacion", 10, an.fechaAutorizacion);
            AddVarchar(cmd, "@nroExpediente", 10, an.nroExpediente);
            AddVarchar(cmd, "@fechaExpediente", 10, an.fechaExpediente);
            AddVarchar(cmd, "@nroResolucion", 10, an.nroResolucion);
            AddVarchar(cmd, "@fechaResolucion", 11, an.fechaResolucion);
            AddVarchar(cmd, "@fechaVigencia", 10, an.fechaVigencia);
            AddVarchar(cmd, "@nroInformeEsp", 10, an.nroInformeEsp);

            AddInt(cmd, "@idSolicitante", an.idSolicitante);
            AddInt(cmd, "@idTipoSolicitud", an.idTipoSolicitud);
            AddInt(cmd, "@idTipoUbicacion", an.idTipoUbicacion);
            AddVarchar(cmd, "@otroUbicacion", 50, an.otroUbicacion);
            AddInt(cmd, "@idTipoElemento", an.idTipoElemento);
            AddVarchar(cmd, "@otroElemento", 50, an.otroElemento);
            AddInt(cmd, "@idTipoCaracteristica", an.idTipoCaracteristica);
            AddVarchar(cmd, "@otroTipoCaracteristica", 50, an.otroTipoCaracteristica);
            AddInt(cmd, "@idTipoEstructura", an.idTipoEstructura);
            AddVarchar(cmd, "@otroTipoEstructura", 50, an.otroTipoEstructura);
            AddInt(cmd, "@idTipoMaterial", an.idTipoMaterial);
            AddVarchar(cmd, "@otroTipoMaterial", 50, an.otroTipoMaterial);

            AddInt(cmd, "@estado", an.estado);
            AddInt(cmd, "@idEstadoTramite", an.idEstadoTramite);
            AddVarchar(cmd, "@operadorRegistra", 10, an.operadorRegistra);
            AddVarchar(cmd, "@estacionRegistra", 20, an.estacionRegistra);
            AddDateTime2(cmd, "@fechaRegistra", an.fechaRegistra);
            AddVarchar(cmd, "@operadoModifica", 10, an.operadoModifica);
            AddVarchar(cmd, "@estacionModifica", 9, an.estacionModifica);
            AddDateTime2(cmd, "@fechaModifica", an.fechaModifica);
            


            // --------- outputs ----------
            var pOutDet = new SqlParameter("@out_idDetalle", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pOutAn = new SqlParameter("@out_idAnuncio", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pOutDet);
            cmd.Parameters.Add(pOutAn);

            await cn.OpenAsync(ct);
            await cmd.ExecuteNonQueryAsync(ct);

            return new CrearAnuncioResult {
                idDetalle = (pOutDet.Value is DBNull) ? 0 : (int)pOutDet.Value,
                idAnuncio = (pOutAn.Value is DBNull) ? 0 : (int)pOutAn.Value
            };
        }

        public async Task<List<ComboDTO>> lstRequisitoEPE(string idTipo, string campo42, string campo43) { 
            var lista = new List<ComboDTO>();

            using(var con = new SqlConnection(_conn))
            using(var cmd = new SqlCommand("Publicidad.lstRequisitosEPE", con)) {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idTipo", idTipo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@campo42", campo42 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@campo43", campo43 ?? (object)DBNull.Value);

                await con.OpenAsync();
                using var dr = await cmd.ExecuteReaderAsync();
                while(await dr.ReadAsync()) {
                    lista.Add(new ComboDTO {
                        Id = dr.GetInt32(dr.GetOrdinal("IdRequisito")),
                        Nombre = dr["Descripcion"].ToString(),
                        Estado = dr.GetBoolean(dr.GetOrdinal("EsActivo")) ? 1 : 0
                    });
                }
            }

            return lista;
        }

        public async Task<string> saveRequisitosEPE(SolicitudRequisitosDTO dto) {
            try {
                using var con = new SqlConnection(_conn);
                using var cmd = new SqlCommand("Publicidad.usp_SolicitudRequisitos_Guardar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAnuncio", dto.IdAnuncio);

                var table = new DataTable();
                table.Columns.Add("IdRequisito", typeof(int));
                table.Columns.Add("chkEstado", typeof(int));
                table.Columns.Add("Descripcion", typeof(string));

                foreach(var r in dto.Requisitos)
                    table.Rows.Add(r.IdRequisito, r.chkEstado, r.Descripcion);

                var tvp = cmd.Parameters.AddWithValue("@Requisitos", table);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "Publicidad.RequisitoSolicitudType";

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return "Requisitos guardados";
            }
            catch(SqlException ex) {
                throw new Exception($"Error SQL al guardar requisitos: {ex.Message}", ex);
            }
            catch(Exception ex) {
                throw new Exception($"Error general al guardar requisitos: {ex.Message}", ex);
            }
        }


        public async Task<bool> UpdateAnuncioOpcion1Async(uptResolucionEPE dto, CancellationToken ct = default) {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("[Publicidad].[usp_anuncio_update]", cn) {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };

            // obligatorios
            cmd.Parameters.Add("@idAnuncio", SqlDbType.Int).Value = dto.idAnuncio;
            cmd.Parameters.Add("@opcion", SqlDbType.TinyInt).Value = 1;

            // helper local para DBNull
            static object Db(object? v) => v ?? DBNull.Value;

            // documentos (solo se actualizan si NO son null)
            cmd.Parameters.Add("@nroExpediente", SqlDbType.VarChar, 10).Value = Db(dto.NroExpediente);
            cmd.Parameters.Add("@fechaExpediente", SqlDbType.VarChar, 10).Value = Db(dto.FechaExpediente);
            cmd.Parameters.Add("@nroAutorizacion", SqlDbType.VarChar, 11).Value = Db(dto.NroAutorizacion);
            cmd.Parameters.Add("@fechaAutorizacion", SqlDbType.VarChar, 10).Value = Db(dto.FechaAutorizacion);
            cmd.Parameters.Add("@fechaVigencia", SqlDbType.VarChar, 10).Value = Db(dto.FechaVigencia);
            cmd.Parameters.Add("@nroInformeEsp", SqlDbType.VarChar, 10).Value = Db(dto.NroInformeEsp);

            // estado/trámite (opcional)
            cmd.Parameters.Add("@idEstadoTramite", SqlDbType.Int).Value = (object?)dto.IdEstadoTramite ?? DBNull.Value;
            cmd.Parameters.Add("@estado", SqlDbType.TinyInt).Value = DBNull.Value; // no se usa en opción 1

            // auditoría — ¡OJO! el parámetro del SP se llama **@operadoModifica** (sin “r”)
            cmd.Parameters.Add("@operadoModifica", SqlDbType.VarChar, 10).Value = Db(dto.OperadorModifica);
            cmd.Parameters.Add("@estacionModifica", SqlDbType.VarChar, 9).Value = Db(dto.EstacionModifica);

            await cn.OpenAsync(ct);

            // Si tu SP mantiene SET NOCOUNT ON, ExecuteNonQuery devolverá -1.
            // Si quieres saber filas afectadas, agrega en el SP: SELECT @@ROWCOUNT AS rowsAffected; y lee con ExecuteScalar/Reader.
            var affected = await cmd.ExecuteNonQueryAsync(ct);

            // devuelve true si no explotó; si agregas SELECT @@ROWCOUNT, cambia la lógica y revisa el valor.
            return affected >= 0 || affected == -1;
        }

        public async Task<string> updateEstadoTramite(string id) {
            var q = "select* from Licencia.EstadoTramite where idEstado in (8, 11, 13, 28)";
            return null;
        }

        /*--------------------------------ANEXOS------------------------*/
        //insertar doc firmado licencia
        public async Task AddDocAnexoPub(AnexoDTO a, IFormFile f) {

            if(f is null || f.Length == 0)
                throw new ArgumentException("El archivo está vacío.", nameof(f));

            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("[Publicidad].[usp_insert_doc_solicitudAnexos]", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@idSolicitud", a.IdSolicitud);
            cmd.Parameters.AddWithValue("@id_anexos_doc", a.IdAnexosDoc);
            cmd.Parameters.AddWithValue("@id_documento", 0);
            cmd.Parameters.AddWithValue("@numero_doc", a.NumeroDoc);
            cmd.Parameters.AddWithValue("@fecha_doc", a.FechaDoc);
            cmd.Parameters.AddWithValue("@siglas_doc", a.SiglasDoc);
            cmd.Parameters.AddWithValue("@operador_registro", a.OperadorRegistro);
            cmd.Parameters.AddWithValue("@estacion_registro", "");

            //Leer IFormFile -> byte[]
            await using var ms = new MemoryStream();
            await f.CopyToAsync(ms);
            var bytes = ms.ToArray();

            // Binario: mejor tiparlo explícito (en vez de AddWithValue)
            var p = cmd.Parameters.Add("@RutaDoc", SqlDbType.VarBinary, -1);
            p.Value = bytes;

           //cmd.Parameters.AddWithValue("@RutaDoc", bytes);
            cmd.Parameters.AddWithValue("@NombreDoc", Path.GetFileName(f.FileName));
            cmd.Parameters.AddWithValue("@flag_valida", a.FlagValida);

            await cmd.ExecuteNonQueryAsync();
        }

        //cambiar estado anexo
        public async Task<bool> DeshabilitarAnexo(int idAnexo, int estado) {
            using(var connection = new SqlConnection(_conn))
            using(var command = new SqlCommand()) {
                command.Connection = connection;
                command.CommandText = @"UPDATE Publicidad.Doc_Solicitud_Anexos_Pub
                                SET flag_estado = @estado 
                                WHERE id_doc_sol_anexos = @idAnexo";

                command.Parameters.AddWithValue("@estado", estado);
                command.Parameters.AddWithValue("@idAnexo", idAnexo);

                await connection.OpenAsync();

                int filasAfectadas = await command.ExecuteNonQueryAsync();

                return filasAfectadas > 0;
            }
        }



    }
}
