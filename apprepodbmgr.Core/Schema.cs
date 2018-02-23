//
//  Author:
//    Natalia Portillo claunia@claunia.com
//
//  Copyright (c) 2017, © Claunia.com
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace apprepodbmgr.Core
{
    public static class Schema
    {
        public const string FilesTableSql = "-- -----------------------------------------------------\n"               +
                                            "-- Table `files`\n"                                                       +
                                            "-- -----------------------------------------------------\n"               +
                                            "DROP TABLE IF EXISTS `files` ;\n\n"                                       +
                                            "CREATE TABLE IF NOT EXISTS `files` (\n"                                   +
                                            "  `id` INTEGER PRIMARY KEY AUTOINCREMENT,\n"                              +
                                            "  `sha256` VARCHAR(64) NOT NULL,\n"                                       +
                                            "  `crack` BOOLEAN NOT NULL,\n"                                            +
                                            "  `hasvirus` BOOLEAN NULL,\n"                                             +
                                            "  `clamtime` DATETIME NULL,\n"                                            +
                                            "  `vtotaltime` DATETIME NULL,\n"                                          +
                                            "  `virus` VARCHAR(128) NULL,\n"                                           +
                                            "  `length` BIGINT NOT NULL);\n\n"                                         +
                                            "CREATE UNIQUE INDEX `files_id_UNIQUE` ON `files` (`id` ASC);\n\n"         +
                                            "CREATE UNIQUE INDEX `files_sha256_UNIQUE` ON `files` (`sha256` ASC);\n\n" +
                                            "CREATE INDEX `files_hasvirus_idx` ON `files` (`hasvirus` ASC);\n\n"       +
                                            "CREATE INDEX `files_virus_idx` ON `files` (`virus` ASC);\n\n"             +
                                            "CREATE INDEX `files_length_idx` ON `files` (`length` ASC);";

        public const string AppsTableSql = "-- -----------------------------------------------------\n"               +
                                           "-- Table `apps`\n"                                                        +
                                           "-- -----------------------------------------------------\n"               +
                                           "DROP TABLE IF EXISTS `apps` ;\n\n"                                        +
                                           "CREATE TABLE IF NOT EXISTS `apps` (\n"                                    +
                                           "  `id` INTEGER PRIMARY KEY AUTOINCREMENT,\n"                              +
                                           "  `mdid` CHAR(40) NOT NULL,\n"                                            +
                                           "  `developer` VARCHAR(45) NOT NULL,\n"                                    +
                                           "  `product` VARCHAR(45) NOT NULL,\n"                                      +
                                           "  `version` VARCHAR(45) NULL,\n"                                          +
                                           "  `languages` VARCHAR(45) NULL,\n"                                        +
                                           "  `architecture` VARCHAR(45) NULL,\n"                                     +
                                           "  `machine` VARCHAR(45) NULL,\n"                                          +
                                           "  `format` VARCHAR(45) NULL,\n"                                           +
                                           "  `description` VARCHAR(45) NULL,\n"                                      +
                                           "  `oem` BOOLEAN NOT NULL,\n"                                              +
                                           "  `upgrade` BOOLEAN NOT NULL,\n"                                          +
                                           "  `update` BOOLEAN NOT NULL,\n"                                           +
                                           "  `source` BOOLEAN NOT NULL,\n"                                           +
                                           "  `files` BOOLEAN NOT NULL,\n"                                            +
                                           "  `netinstall` BOOLEAN NOT NULL,\n"                                       +
                                           "  `xml` BLOB NULL,\n"                                                     +
                                           "  `json` BLOB NULL);\n\n"                                                 +
                                           "CREATE UNIQUE INDEX `apps_id_UNIQUE` ON `apps` (`id` ASC);\n\n"           +
                                           "CREATE UNIQUE INDEX `apps_mdid_UNIQUE` ON `apps` (`mdid` ASC);\n\n"       +
                                           "CREATE INDEX `apps_developer_idx` ON `apps` (`developer` ASC);\n\n"       +
                                           "CREATE INDEX `apps_product_idx` ON `apps` (`product` ASC);\n\n"           +
                                           "CREATE INDEX `apps_version_idx` ON `apps` (`version` ASC);\n\n"           +
                                           "CREATE INDEX `apps_architecture_idx` ON `apps` (`architecture` ASC);\n\n" +
                                           "CREATE INDEX `apps_format_idx` ON `apps` (`format` ASC);\n\n"             +
                                           "CREATE INDEX `apps_machine_idx` ON `apps` (`machine` ASC);\n\n"           +
                                           "CREATE INDEX `apps_description_idx` ON `apps` (`description` ASC);";
    }
}