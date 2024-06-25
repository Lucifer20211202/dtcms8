<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统管理'},{title:'系统设置'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item prop="webName" label="主站名称">
							<el-input v-model="datas.model.webName" placeholder="任意字符，控制在255字符内"></el-input>
						</el-form-item>
						<el-form-item prop="webUrl" label="主站域名">
							<el-input v-model="datas.model.webUrl" placeholder="必填，以http://开头"></el-input>
						</el-form-item>
						<el-form-item prop="webLogo" label="主站LOGO">
							<dt-upload-text v-model="datas.model.webLogo" placeholder="请上传文件" exts="jpg,jpeg,png,gif,webp" :size="1024" />
						</el-form-item>
						<el-form-item prop="webVersion" label="系统版本">
							<el-input v-model="datas.model.webVersion" placeholder="必填，系统版本号"></el-input>
						</el-form-item>
						<el-form-item label="公司名称">
							<el-input v-model="datas.model.webCompany" placeholder="可空，控制在128字符内"></el-input>
						</el-form-item>
						<el-form-item label="通讯地址">
							<el-input v-model="datas.model.webAddress" placeholder="可空，控制在128字符内"></el-input>
						</el-form-item>
						<el-form-item label="联系电话">
							<el-input v-model="datas.model.webTel" placeholder="可空，区号+电话号码"></el-input>
						</el-form-item>
						<el-form-item label="传真号码">
							<el-input v-model="datas.model.webFax" placeholder="可空，区号+传真号码"></el-input>
						</el-form-item>
						<el-form-item label="管理员邮箱">
							<el-input v-model="datas.model.webMail" placeholder="可空，电子邮箱格式"></el-input>
						</el-form-item>
						<el-form-item label="主站备案号">
							<el-input v-model="datas.model.webCrod" placeholder="可空，控制在128字符内"></el-input>
						</el-form-item>
						<el-form-item label="网站运行状态">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.webStatus"></el-switch>
						</el-form-item>
						<el-form-item label="网站关闭原因">
							<el-input type="textarea" :rows="3" v-model="datas.model.webCloseReason" maxlength="512" show-word-limit></el-input>
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="短信平台" name="sms">
					<div class="tab-content">
						<el-form-item prop="smsProvider" label="短信服务商">
							<el-radio-group v-model="datas.model.smsProvider">
								<el-radio-button :label="1">阿里云</el-radio-button>
								<el-radio-button :label="2">腾讯云</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="smsSecretId" label="AccessKey">
							<el-input v-model="datas.model.smsSecretId" placeholder="AccessKey ID"></el-input>
						</el-form-item>
						<el-form-item prop="smsSecretKey" label="SecretKey">
							<el-input v-model="datas.model.smsSecretKey" placeholder="AccessKey Secret"></el-input>
						</el-form-item>
						<el-form-item prop="smsAppId" v-if="datas.model.smsProvider===2" label="短信SdkAppId">
							<el-input v-model="datas.model.smsAppId" placeholder="短信SDKAPPID,示例值：1400006666"></el-input>
						</el-form-item>
						<el-form-item prop="smsSignTxt" label="短信签名">
							<el-input v-model="datas.model.smsSignTxt" placeholder="已报备通过的短信签名"></el-input>
						</el-form-item>
						<el-form-item label="短信测试">
							<el-button :loading="datas.smsloading" @click="datas.smsShowDialog=true">发送测试短信</el-button>
						</el-form-item>
						<el-form-item label="填写说明">
							<div class="remark">
								短信服务商的不同，模板的格式不同，请注意修改系统通知模板；<br />
								短信的签名、模板，请在相关的平台设置和获取，审核后才可使用；<br />
								修改短信账户信息后，请保存成功后再进行发送测试短信；<br />
								测试短信的模板变量，阿里云格式如：{"参数名","值"}，腾讯云如：值1,值2
							</div>
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="邮箱设置" name="email">
					<div class="tab-content">
						<el-form-item label="SSL加密连接">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.emailSSL"></el-switch>
						</el-form-item>
						<el-form-item prop="emailSmtp" label="STMP服务器">
							<el-input v-model="datas.model.emailSmtp" placeholder="发送邮件的SMTP服务器地址"></el-input>
						</el-form-item>
						<el-form-item prop="emailPort" label="SMTP端口">
							<el-input v-model="datas.model.emailPort" placeholder="SMTP服务器端口，一般是25"></el-input>
						</el-form-item>
						<el-form-item prop="emailFrom" label="发件人地址">
							<el-input v-model="datas.model.emailFrom" placeholder="发件人邮箱地址"></el-input>
						</el-form-item>
						<el-form-item prop="emailUserName" label="邮箱账号">
							<el-input v-model="datas.model.emailUserName"></el-input>
						</el-form-item>
						<el-form-item prop="emailPassword" label="邮箱密码">
							<el-input show-password v-model="datas.model.emailPassword"></el-input>
						</el-form-item>
						<el-form-item prop="emailNickname" label="发件人昵称">
							<el-input v-model="datas.model.emailNickname" placeholder="邮件里显示的昵称"></el-input>
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="上传设置" name="upload">
					<div class="tab-content">
						<el-form-item prop="fileServer" label="文件服务器">
							<el-radio-group v-model="datas.model.fileServer">
								<el-radio-button label="localhost">本地存储</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="filePath" label="上传目录名">
							<el-input v-model="datas.model.filePath" placeholder="文件夹目录名称"></el-input>
						</el-form-item>
						<el-form-item prop="fileSave" label="文件保存方式">
							<el-select v-model="datas.model.fileSave" placeholder="请选择...">
								<el-option label="按年月日每天一个目录" :value="1"></el-option>
								<el-option label="按年月/日/存入不同目录" :value="2"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item label="编辑器图片">
							<el-radio-group v-model="datas.model.fileRemote">
								<el-radio-button label="0">禁止下载</el-radio-button>
								<el-radio-button label="1">自动下载</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="fileExtension" label="文件上传类型">
							<el-input v-model="datas.model.fileExtension" placeholder="以英文逗号分隔开,如:“jpg,png”"></el-input>
						</el-form-item>
						<el-form-item prop="videoExtension" label="视频上传类型">
							<el-input v-model="datas.model.videoExtension" placeholder="以英文逗号分隔开,如:“mp4,flv”"></el-input>
						</el-form-item>
						<el-form-item prop="attachSize" label="附件上传大小">
							<el-input v-model="datas.model.attachSize" placeholder="单位KB，0不限制">
								<template #append>KB</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="videoSize" label="视频上传大小">
							<el-input v-model="datas.model.videoSize" placeholder="单位KB，0不限制">
								<template #append>KB</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="imgSize" label="图片上传大小">
							<el-input v-model="datas.model.imgSize" placeholder="单位KB，0不限制">
								<template #append>KB</template>
							</el-input>
						</el-form-item>
						<el-form-item label="图片最大尺寸">
							<el-col :span="11">
								<el-form-item prop="imgMaxHeight">
									<el-input v-model="datas.model.imgMaxHeight" style="width:100%;" placeholder="高度">
										<template #append>高/px</template>
									</el-input>
								</el-form-item>
							</el-col>
							<el-col class="line" :span="2">*</el-col>
							<el-col :span="11">
								<el-form-item prop="imgMaxWidth">
									<el-input v-model="datas.model.imgMaxWidth" style="width:100%;" placeholder="宽度">
										<template #append>宽/px</template>
									</el-input>
								</el-form-item>
							</el-col>
						</el-form-item>
						<el-form-item label="缩略图尺寸">
							<el-col :span="11">
								<el-form-item prop="thumbnailHeight">
									<el-input v-model="datas.model.thumbnailHeight" style="width:100%;" placeholder="高度">
										<template #append>高/px</template>
									</el-input>
								</el-form-item>
							</el-col>
							<el-col class="line" :span="2">*</el-col>
							<el-col :span="11">
								<el-form-item prop="thumbnailWidth">
									<el-input v-model="datas.model.thumbnailWidth" style="width:100%;" placeholder="宽度">
										<template #append>宽/px</template>
									</el-input>
								</el-form-item>
							</el-col>
						</el-form-item>
						<el-form-item prop="thumbnailMode" label="缩略图生成方式">
							<el-radio-group v-model="datas.model.thumbnailMode">
								<el-radio-button label="Cut">中心裁剪</el-radio-button>
								<el-radio-button label="HW">两边补白</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="watermarkType" label="图片水印类型">
							<el-radio-group v-model="datas.model.watermarkType">
								<el-radio-button label="0">不启用</el-radio-button>
								<el-radio-button label="1">文字水印</el-radio-button>
								<el-radio-button label="2">图片水印</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="watermarkPosition" label="图片水印位置">
							<el-select v-model="datas.model.watermarkPosition" placeholder="请选择...">
								<el-option label="左上" :value="1"></el-option>
								<el-option label="中上" :value="2"></el-option>
								<el-option label="右上" :value="3"></el-option>
								<el-option label="左中" :value="4"></el-option>
								<el-option label="居中" :value="5"></el-option>
								<el-option label="右中" :value="6"></el-option>
								<el-option label="左下" :value="7"></el-option>
								<el-option label="中下" :value="8"></el-option>
								<el-option label="右下" :value="9"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="watermarkPic" label="图片水印文件">
							<el-input v-model="datas.model.watermarkPic" placeholder="需存放wwwroot根目录下,没有使用文字水印"></el-input>
						</el-form-item>
						<el-form-item label="图片生成质量">
							<el-slider v-model="datas.model.watermarkImgQuality" show-input></el-slider>
						</el-form-item>
						<el-form-item label="水印透明度">
							<el-slider v-model="datas.model.watermarkTransparency" show-input :max="10"></el-slider>
						</el-form-item>
						<el-form-item prop="watermarkFont" label="水印字体">
							<el-select v-model="datas.model.watermarkFont" placeholder="请选择...">
								<el-option label="Arial" value="Arial"></el-option>
								<el-option label="Symbol" value="Symbol"></el-option>
								<el-option label="Tahoma" value="Tahoma"></el-option>
								<el-option label="Verdana" value="Verdana"></el-option>
								<el-option label="仿宋_GB2312" value="仿宋_GB2312"></el-option>
								<el-option label="宋体" value="宋体"></el-option>
								<el-option label="新宋体" value="新宋体"></el-option>
								<el-option label="楷体_GB2312" value="楷体_GB2312"></el-option>
								<el-option label="微软雅黑" value="微软雅黑"></el-option>
								<el-option label="黑体" value="黑体"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="watermarkText" label="水印文字">
							<el-input v-model="datas.model.watermarkText" placeholder="文字水印的内容"></el-input>
						</el-form-item>
						<el-form-item prop="watermarkFontSize" label="文字大小px">
							<el-input v-model="datas.model.watermarkFontSize" placeholder="文字水印的大小">
								<template slot="append">
									px
								</template>
							</el-input>
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
		
		<el-dialog v-model="datas.smsShowDialog" title="发送测试短信" destroy-on-close append-to-body>
			<div class="dialog-box">
				<dt-form-box ref="sendFormRef" v-model="datas.smsModel" :rules="datas.smsRules" activeName="info">
					<el-tab-pane label="字段信息" name="info">
						<div class="tab-content">
							<el-form-item prop="phoneNumbers" label="手机号码">
								<el-input v-model="datas.smsModel.phoneNumbers" placeholder="多个手机号以英文逗号分隔"></el-input>
							</el-form-item>
							<el-form-item prop="templateId" label="模板标识">
								<el-input v-model="datas.smsModel.templateId" placeholder="已报备的模板标识"></el-input>
							</el-form-item>
							<el-form-item prop="templateParam" label="模板变量">
								<el-input v-model="datas.smsModel.templateParam" placeholder="阿里云是JSON格式,腾讯云以逗号分隔"></el-input>
							</el-form-item>
						</div>
					</el-tab-pane>
				</dt-form-box>
			</div>
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" icon="Finished" @click="sendSmsMessage">确认发送</el-button>
					<el-button type="warning" @click="datas.smsShowDialog = false">关闭</el-button>
				</div>
			</template>
		</el-dialog>
		
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../components/layout/DtFormBox.vue'
	import DtUploadText from '../../components/upload/DtUploadText.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	const editFormRef = ref(null)
	const sendFormRef = ref(null)
	
	//定义表单属性
	const datas = reactive({
		loading: false,
		smsloading: false,
		smsShowDialog: false,
		smsModel: {
			phoneNumbers: null,
			templateId: null,
			templateParam: null,
		},
		smsRules: {
			phoneNumbers: [
				{ required: true, message: '请填写发送短信手机号码', trigger: 'blur' }
			],
			templateId: [
				{ required: true, message: '请填写短信模板标识', trigger: 'blur' }
			],
			templateParam: [
				{ required: true, message: '请填写短信模板变量', trigger: 'blur' }
			],
		},
		model: {
			webName: null,
			webUrl: null,
			webLogo: null,
			webVersion: null,
			webCompany: null,
			webAddress: null,
			webTel: null,
			webFax: null,
			webMail: null,
			webCrod: null,
			webStatus: 0,
			webCloseReason: null,
			smsProvider: null,
			smsSecretId: null,
			smsSecretKey: null,
			smsSignTxt: null,
			smsAppId: null,
			emailSmtp: null,
			emailSSL: 0,
			emailPort: 0,
			emailFrom: null,
			emailUserName: null,
			emailPassword: null,
			emailNickname: null,
			filePath: null,
			fileSave: 0,
			fileRemote: 0,
			fileExtension: null,
			videoExtension: null,
			attachSize: 0,
			videoSize: 0,
			imgSize: 0,
			imgMaxHeight: 0,
			imgMaxWidth: 0,
			thumbnailHeight: 0,
			thumbnailWidth: 0,
			thumbnailMode: null,
			watermarkType: 0,
			watermarkPosition: 0,
			watermarkImgQuality: 0,
			watermarkPic: null,
			watermarkTransparency: 0,
			watermarkText: null,
			watermarkFont: null,
			watermarkFontSize: 0,
			fileServer: null,
		},
		rules: {
			webName: [
				{ required: true, message: '请填写主站的名称', trigger: 'blur' }
			],
			webUrl: [
				{ required: true, message: '请填写主站的域名', trigger: 'blur' }
			],
			webVersion: [
				{ required: true, message: '请填写系统版本号', trigger: 'blur' }
			],
			smsSecretId: [
				{ required: true, message: '请填写服务商的AccessKey ID', trigger: 'blur' }
			],
			smsSecretKey: [
				{ required: true, message: '请填写服务商的AccessKey Secret', trigger: 'blur' }
			],
			smsAppId: [
				{ required: true, message: '请填写服务商的SdkAppId', trigger: 'blur' }
			],
			smsSignTxt: [
				{ required: true, message: '请填写已报备的短信签名', trigger: 'blur' }
			],
			emailSmtp: [
				{ required: true, message: '请填写邮箱SMTP服务器地址', trigger: 'blur' }
			],
			emailPort: [
				{ required: true, message: '请填写邮箱SMTP服务器端口', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			emailFrom: [
				{ required: true, message: '请填写邮箱发件人地址', trigger: 'blur' }
			],
			emailUserName: [
				{ required: true, message: '请填写邮箱账号', trigger: 'blur' }
			],
			emailPassword: [
				{ required: true, message: '请填写邮箱密码', trigger: 'blur' }
			],
			emailNickname: [
				{ required: true, message: '请填写发件人昵称', trigger: 'blur' }
			],
			filePath: [
				{ required: true, message: '请填写上传目录名', trigger: 'blur' }
			],
			fileSave: [
				{ required: true, message: '请选择文件保存方式', trigger: 'change' }
			],
			fileExtension: [
				{ required: true, message: '请填写允许文件上传扩展名', trigger: 'blur' }
			],
			videoExtension: [
				{ required: true, message: '请填写允许视频上传扩展名', trigger: 'blur' }
			],
			attachSize: [
				{ required: true, message: '请填写附件上传大小KB', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			videoSize: [
				{ required: true, message: '请填写视频上传大小KB', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			imgSize: [
				{ required: true, message: '请填写图片上传大小KB', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			imgMaxWidth: [
				{ required: true, message: '请填写图片最大宽度', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			imgMaxHeight: [
				{ required: true, message: '请填写图片最大高度', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			thumbnailWidth: [
				{ required: true, message: '请填写缩略图片宽度', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			thumbnailHeight: [
				{ required: true, message: '请填写缩略图片高度', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			thumbnailMode: [
				{ required: true, message: '请选择缩略图生成方式', trigger: 'change' }
			],
			watermarkType: [
				{ required: true, message: '请选择图片水印类型', trigger: 'change' }
			],
			watermarkPosition: [
				{ required: true, message: '请选择图片水印位置', trigger: 'change' }
			],
			watermarkPic: [
				{ required: true, message: '请填写图片水印文件路径', trigger: 'blur' }
			],
			watermarkText: [
				{ required: true, message: '请填写文字水印内容', trigger: 'blur' }
			],
			watermarkFont: [
				{ required: true, message: '请选择水印字体', trigger: 'change' }
			],
			watermarkFontSize: [
				{ required: true, message: '请填写水印的文字大小', trigger: 'blur' }
			],
			imageSize: [
				{ required: true, message: '请填写尺寸大小', trigger: 'blur' },
				{ pattern: /^[0-9]*[1-9][0-9]*$/, message: '只能输入整数', trigger: 'blur' }
			],
			fileServer: [
				{ required: true, message: '请选择文件存储服务器', trigger: 'change' }
			],
		}
	})
	
	//初始化数据
	const initData = async() => {
		//获取分类列表
		await proxy.$api.request({
		    url: `/admin/setting/sysconfig`,
		    success(res) {
				datas.model = res.data
		    }
		});
	}
	//提交表单
	const submitForm = () => {
		editFormRef.value.form.validate((valid) => {
			if (valid) {
				proxy.$api.request({
					method: 'put',
					url: '/admin/setting/sysconfig',
					data: datas.model,
					successMsg: '系统设置已成功修改',
					beforeSend() {
						datas.loading = true
					},
					success(res) {
						//可写回调处理
					},
					complete() {
						datas.loading = false
					}
				})
			} else {
				proxy.$message({ type: 'warning', message: '表单验证失败，请检查后重试' })
				return false
			}
		})
	}
	//发送测试短信
	const sendSmsMessage = () => {
		if (!sendFormRef.value.form) return
		sendFormRef.value.form.validate((valid) => {
			if (valid) {
				proxy.$api.request({
					method: 'post',
					url: '/admin/setting/sms/account/test',
					data: datas.smsModel,
					successMsg: '短信已提交成功',
					beforeSend() {
						datas.smsloading = true
					},
					success(res) {
						datas.smsShowDialog = false
					},
					complete() {
						datas.smsloading = false
					}
				})
			}
		})
	}
	
	//执行方法
	initData()
</script>