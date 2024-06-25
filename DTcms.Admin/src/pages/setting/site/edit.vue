<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统管理'},{title:'编辑站点'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="first">
				<el-tab-pane label="站点设置" name="first">
					<div class="tab-content">
						<el-form-item label="是否启用">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item label="是否默认">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isDefault"></el-switch>
						</el-form-item>
						<el-form-item prop="name" label="站点标识">
							<el-input v-model="datas.model.name" placeholder="英文字母,站点URL路由标识"></el-input>
						</el-form-item>
						<el-form-item prop="dirPath" label="模板目录名">
							<el-input v-model="datas.model.dirPath" placeholder="对应模板目录名"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item label="创建时间" v-if="datas.model.id>0">
							{{datas.model.addBy}}  {{datas.model.addTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="网站信息" name="info">
					<div class="tab-content">
						<el-form-item prop="title" label="网站标题">
							<el-input v-model="datas.model.title" placeholder="任意字符，255个字符内"></el-input>
						</el-form-item>
						<el-form-item label="网站LOGO">
							<dt-upload-image v-model="datas.model.logo"></dt-upload-image>
						</el-form-item>
						<el-form-item label="单位名称">
							<el-input v-model="datas.model.company"></el-input>
						</el-form-item>
						<el-form-item label="单位地址">
							<el-input v-model="datas.model.address"></el-input>
						</el-form-item>
						<el-form-item label="联系电话">
							<el-input v-model="datas.model.tel" placeholder="非必填，区号+电话号码"></el-input>
						</el-form-item>
						<el-form-item label="传真号码">
							<el-input v-model="datas.model.fax" placeholder="非必填，区号+传真号码"></el-input>
						</el-form-item>
						<el-form-item label="电子邮箱">
							<el-input v-model="datas.model.email"></el-input>
						</el-form-item>
						<el-form-item label="网站备案号">
							<el-input v-model="datas.model.crod"></el-input>
						</el-form-item>
						<el-form-item label="首页SEO标题">
							<el-input v-model="datas.model.seoTitle" placeholder="自定义的首页标题"></el-input>
						</el-form-item>
						<el-form-item label="页面关健词">
							<el-input v-model="datas.model.seoKeyword" placeholder="页面关键词(keyword)"></el-input>
						</el-form-item>
						<el-form-item label="页面描述">
							<el-input v-model="datas.model.seoDescription" placeholder="页面描述(description)"></el-input>
						</el-form-item>
						<el-form-item label="版权信息">
							<el-input type="textarea" :rows="3" v-model="datas.model.copyright" maxlength="1024" show-word-limit></el-input>
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="绑定域名" name="domain">
					<div class="tab-content">
						<el-row>
							<el-col :span="24">
								<el-button type="primary" icon="Plus" @click="addDomain">添加</el-button>
								<el-button type="danger" icon="Delete" @click="removeDomain">删除</el-button>
							</el-col>
						</el-row>
						<el-card class="table-card mat-20">
							<el-table ref="domainTableRef" :data="datas.model.domains" class="table-form" @selection-change="handleSelectionChange">
								<el-table-column type="selection" width="55"></el-table-column>
								<el-table-column width="280" label="域名">
									<template #default="scope">
										<el-form-item :prop="'domains.' + scope.$index + '.domain'" :rules='datas.rules.domain' label-width="0px" style="margin:auto">
											<el-input v-model="scope.row.domain" placeholder="不含http://部分"></el-input>
										</el-form-item>
									</template>
								</el-table-column>
								<el-table-column min-width="180" label="备注">
									<template #default="scope">
										<el-form-item :prop="'domains.' + scope.$index + '.remark'" label-width="0px" style="margin:auto">
											<el-input v-model="scope.row.remark"></el-input>
										</el-form-item>
									</template>
								</el-table-column>
								<el-table-column fixed="right" width="80" label="操作" align="center">
									<template #default="scope">
										<el-link :underline="false" icon="Delete" @click="deleteDomain(scope.$index)"></el-link>
									</template>
								</el-table-column>
							</el-table>
						</el-card>
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
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	import DtUploadImage from '../../../components/upload/DtUploadImage.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//通知父组件响应
	const emit = defineEmits(['loadMenu'])
	//接收props传值
	const props = defineProps({
		id: 0
	})
	const editFormRef = ref(null)
	const domainTableRef = ref(null)
	
	//定义表单属性
	const datas = reactive({
		loading: false,
		multipleSelection: [],
		model: {
			id: 0,
			name: '',
			title: '',
			dirPath: '',
			isDefault: 0,
			logo: '',
			company: '',
			address: '',
			tel: '',
			fax: '',
			email: '',
			crod: '',
			copyright: '',
			seoTitle: '',
			seoKeyword: '',
			seoDescription: '',
			sortId: 99,
			status: 0,
			domains: []
		},
		rules: {
			name: [
				{ required: true, message: '请输入站点名称', trigger: 'blur' },
				{ min: 2, max: 128, message: '长度在 2 到 128 个字符', trigger: 'blur' },
				{ pattern: /^[a-zA-Z_]{1,}$/, message: '只能字母和下划线', trigger: 'blur' }
			],
			title: [
				{ required: true, message: '请输入网站标题', trigger: 'blur' },
				{ min: 2, max: 128, message: '长度在 2 到 128 个字符', trigger: 'blur' }
			],
			dirPath: [
				{ required: true, message: '请输入模板目录名', trigger: 'blur' },
				{ pattern: /^[0-9a-zA-Z_]{1,}$/, message: '只能字母数字下划线', trigger: 'blur' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' }
			],
			domain: [
				{ required: true, message: '请填写绑定的域名', trigger: 'blur' }
			]
		}
	})
	
	//初始化数据
	const initData = async() => {
		//赋值表单
		if (props.id) {
			await proxy.$api.request({
				url: '/admin/site/' + props.id,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/site/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改站点已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							emit('loadMenu') //通知父组件重新加载菜单
							proxy.$common.linkUrl('/setting/site/list') //跳转列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/site',
						data: datas.model,
						successMsg: '新增站点已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
							emit('loadMenu') //通知父组件重新加载菜单
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	//删除选中的域名
	const removeDomain = () => {
		let list = datas.multipleSelection //拿到选中的数据
		if (list.length) {
			list.forEach((item, index) => {
				//遍历源数据
				datas.model.domains.forEach((v, i) => {
					//如果选中数据和源数据的某一条唯一标识符相等，删除对应的源数据
					if (item == v) {
						datas.model.domains.splice(i, 1)
					}
				})
			});
		}
		//清除选中状态
		domainTableRef.value.clearSelection()
	}
	//删除一行域名
	const deleteDomain = (index) => {
		datas.model.domains.splice(index, 1)
	}
	//添加域名
	const addDomain = () => {
		datas.model.domains.push({
			id: 0,
			siteId: 0,
			domain: null,
			remark: null
		});
	}
	//选中项
	const handleSelectionChange = (val) => {
		datas.multipleSelection = val
	}
	
	//运行初始化
	initData()
</script>